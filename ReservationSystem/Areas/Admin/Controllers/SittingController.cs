using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Areas.Admin.Models.Sitting;
using ReservationSystem.Data;
using ReservationSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReservationSystem.Areas.Admin.Models.Sitting.DetailsSitting;

namespace ReservationSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SittingController : AdminAreaBaseController
    {
        #region DI
        private readonly IMapper _mapper;
        public SittingController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager, IMapper mapper) : base(cxt, userManager)
        {
            _mapper = mapper;
        }
        #endregion

        #region ACTION METHODS
        public IActionResult IndexSitting()
        {
            return View();
        }

        public async Task<IActionResult> PastSittings()
        {
            var pastSittings = GetSittings().Where(s => s.Date < DateTime.Today);
            foreach(var s in pastSittings)
            {
                if (s.Status != Data.Enums.SittingStatus.Past)
                {
                    s.Status = Data.Enums.SittingStatus.Past;
                }
            }
           await _cxt.SaveChangesAsync();
            var pastSittingsList = await GetSittings().Where(s => s.Date < DateTime.Today).OrderBy(s => s.Date).ToListAsync();
            return View(pastSittingsList);
        }

        public async Task<IActionResult> FutureSittings()
        {
            var futureSittings = await GetSittings().Where(s => s.Date >= DateTime.Today).OrderBy(s => s.Date).ToListAsync();
            return View(futureSittings);
        }

        public async Task<IActionResult> DetailsSitting(int id)
        {
            var m = new DetailsSitting
            {
                Sitting = await _cxt.Sittings
                    .Include(s => s.Reservations).ThenInclude(r => r.Customer)
                    .Include(s => s.SittingCategory).ThenInclude(sc => sc.SCTables.OrderBy(t => t.Table.Id)).ThenInclude(sct => sct.Table)
                    .Include(s => s.SittingCategory).ThenInclude(sc => sc.SCTimeslots.OrderBy(t => t.StartTime))
                    .Include(s => s.SittingUnits)
                    .FirstOrDefaultAsync(s => s.Id == id),
            };
            foreach (var scts in m.Sitting.SittingCategory.SCTimeslots)
            {
                m.SCTimeslotsDTO.Add(new SCTimeslotDTO
                {
                    Id = scts.Id,
                    StartTime = scts.StartTime.ToString(@"hh\:mm\:ss"),
                    EndTime = scts.EndTime.ToString(@"hh\:mm\:ss"),
                });
            }

            foreach (var sct in m.Sitting.SittingCategory.SCTables)
            {
                m.SCTablesDTO.Add(new SCTableDTO
                {
                    Id = sct.Id,
                    Name = sct.Table.Name,
                    Area = sct.Table.Area,
                });
            }

            foreach (var fsu in m.Sitting.SittingUnits)
            {
                m.SittingUnitsDTO.Add(new SittingUnitDTO
                {
                    Id = fsu.Id,
                    TableId = fsu.TableId,
                    TimeslotId = fsu.TimeslotId,
                    ReservationId = fsu.ReservationId,
                });
            }
            return View(m);
        }

        [HttpGet]
        public async Task<IActionResult> CreateSitting(int id)
        {
            var m = new CreateSitting();
            var sittingCategory = await _cxt.SittingCategories.FirstOrDefaultAsync(sc => sc.Id == id);
            m.SittingCategoryId = sittingCategory.Id;
            m.SittingCategory = sittingCategory;
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSitting(Models.Sitting.CreateSitting m)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (m.StartDate > m.EndDate)
                    {
                        return StatusCode(403);
                    }

                    var sittingCategory = await _cxt.SittingCategories.FirstOrDefaultAsync(sc => sc.Id == m.SittingCategoryId);
                    var sittings = new List<Sitting>();
                    DateTime date = m.StartDate;
                    var invalidDates = new List<string>();

                    while (date <= m.EndDate)
                    {
                        if (SCSelectionValidation(date, m.SittingCategoryId))
                        {
                            sittings.Add(new Sitting { SittingCategoryId = m.SittingCategoryId, Date = date, Status = SittingStatus.Open });
                        }
                        else
                        {
                            invalidDates.Add(date.ToShortDateString());
                        }
                        date = date.AddDays(1);
                    }

                    //display warning message for overlapping dates
                    if (invalidDates.Count > 0)
                    {
                        TempData["Message"] = string.Format("Overlapping sitting(s) on {0} not created for Category: {1}.",                            
                            string.Join(", ", values: invalidDates),
                            sittingCategory.Name);
                    }

                    _cxt.Sittings.AddRange(sittings);
                    await _cxt.SaveChangesAsync();

                    //CreateSittingUnits();
                    int sittingId = await _cxt.Sittings.MaxAsync(s => s.Id) - sittings.Count + 1;
                    for (int i = 0; i < sittings.Count; i++)
                    {
                        await CreateSittingUnits(sittingId, m.SittingCategoryId);
                        sittingId++;
                    }

                    return RedirectToAction("DetailsSC", "SittingCategory", new { id = m.SittingCategoryId });
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Invalid submission!");
                }
            }
            return View();
        }

        public async Task<IActionResult> ManuallyCloseSitting(int id)
        {
            //Manually close a sitting
            var sitting = _cxt.Sittings.Include(s => s.Reservations).FirstOrDefault(s => s.Id == id);
            sitting.Status = Data.Enums.SittingStatus.Closed;
            await _cxt.SaveChangesAsync();
            return RedirectToAction(nameof(FutureSittings));
        }

        public async Task<IActionResult> ManuallyReopenSitting(int id)
        {
            //Manually close a sitting
            var sitting = _cxt.Sittings.Include(s => s.Reservations).Include(s => s.SittingCategory).FirstOrDefault(s => s.Id == id);
            if (sitting.RemainingCapacity > 0) { sitting.Status = Data.Enums.SittingStatus.Open; }
            await _cxt.SaveChangesAsync();
            return RedirectToAction(nameof(FutureSittings));
        }
        #endregion

        #region SITTING METHODS
        public Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Sitting, List<SittingUnit>> GetSittings()
        {
            return _cxt.Sittings
                .Include(s => s.SittingCategory)
                .Include(s => s.Reservations).ThenInclude(r => r.Customer)
                .Include(s => s.SittingUnits);
        }

        //find a sitting by id
        //public async Task<Sitting> GetSittingById(int id)
        //{
        //    return await GetSittings().FirstOrDefaultAsync(s => s.Id == id);
        //}

        //check if the sitting to be created overlaps with existing sittings
        public bool SCSelectionValidation(DateTime date, int sittingCategoryId)
        {
            var sittings = _cxt.Sittings.Where(s => s.Date == date).Include(s => s.SittingCategory).ToList();
            if (sittings == null) { return true; }

            var sittingCategory = _cxt.SittingCategories.FirstOrDefault(sc => sc.Id == sittingCategoryId);
            foreach (var sitting in sittings)
            {
                if (!(
                    (sittingCategory.EndTime <= sitting.SittingCategory.StartTime) || (sittingCategory.StartTime >= sitting.SittingCategory.EndTime)
                    ))
                { return false; }
            }
            return true;
        }

        //add sitting units for a sitting to database
        public async Task CreateSittingUnits(int sittingId, int sittingCategoryId)
        {
            var sUnits = new List<SittingUnit>();
            var sc = await _cxt.SittingCategories
                .Include(s => s.SCSittings)
                .Include(s => s.SCTables)
                .Include(s => s.SCTimeslots)
                .FirstOrDefaultAsync(s => s.Id == sittingCategoryId);

            foreach (var scTable in sc.SCTables)
            {
                foreach (var scTimeslot in sc.SCTimeslots)
                {
                    sUnits.Add(new SittingUnit(sittingId, scTimeslot.Id, scTable.Id));
                }
            }
            _cxt.SittingUnits.AddRange(sUnits);

            await _cxt.SaveChangesAsync();
        }
        #endregion
    }
}
