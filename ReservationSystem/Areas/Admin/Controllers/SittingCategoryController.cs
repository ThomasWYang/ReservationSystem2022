using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SittingCategoryController : AdminAreaBaseController
    {
        #region DI
        private readonly IMapper _mapper;
        public SittingCategoryController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager, IMapper mapper) : base(cxt, userManager)
        {
            _mapper = mapper;
        }
        #endregion

        #region ACTION METHODS
        public async Task<ActionResult> IndexSC()
        {
            var sittingCategories = await GetSCs();
            return View(sittingCategories);
        }

        public async Task<ActionResult> DetailsSC(int id)
        {
            var sc = await GetSCById(id);
            GetTableStrings(sc);
            return View(sc);
        }

        [HttpGet]
        public ActionResult CreateSC()
        {
            var selectedTable = _cxt.Tables.Select(t => t.Id).ToArray();
            var m = new Models.SittingCategory.CreateSC
            {
                Tables = new MultiSelectList(_cxt.Tables.ToArray(), nameof(Table.Id), nameof(Table.Name), selectedTable)
            };
            return View(m);
        }

        //check if the SC name entered has been used already
        public async Task<ActionResult> IsNameAvailbleAsync(string name)
        {
            var repeatedNameSC = await _cxt.SittingCategories.FirstOrDefaultAsync(sc => sc.Name == name);
            if (repeatedNameSC == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Name \"{name}\" is already in use.");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateSC(Models.SittingCategory.CreateSC m)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var s = _mapper.Map<SittingCategory>(m);
                    _cxt.SittingCategories.Add(s);
                    await _cxt.SaveChangesAsync();

                    var id = await CreateSCId();
                    await CreateSCTimeslots(m.StartTime, m.Duration, new TimeSpan(m.IntervalHours, m.IntervalMinutes, 0), id);
                    await CreateSCTables(id, m.TablesId);

                    return RedirectToAction(nameof(IndexSC));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "Invalid submission!");
                }
            }
            m.Tables = new MultiSelectList(_cxt.Tables.ToArray(), nameof(Table.Id), nameof(Table.Name));
            return View(m);
        }

        //admin can delete an empty sitting category
        public async Task<ActionResult> DeleteSC(int id)
        {
            var sittingCategoryTBD = _cxt.SittingCategories.FirstOrDefault(sc => sc.Id == id);
            if (_cxt.Sittings.FirstOrDefault(s => s.SittingCategoryId == id) == null)
            {
                var scTimeslotsTBD = _cxt.SCTimeslots.Where(sct => sct.SittingCategoryId == id);
                var scTablesTBD = _cxt.SCTables.Where(sct => sct.SittingCategoryId == id);
                _cxt.SCTimeslots.RemoveRange(scTimeslotsTBD);
                _cxt.SCTables.RemoveRange(scTablesTBD);
                _cxt.SittingCategories.RemoveRange(sittingCategoryTBD);
                await _cxt.SaveChangesAsync();
                return RedirectToAction(nameof(IndexSC));
            }
            else
            {
                return StatusCode(403);
            }
        }
        #endregion

        #region SC METHODS
        //get the id of the newly added SC
        public async Task<int> CreateSCId()
        {
            return await _cxt.SittingCategories.MaxAsync(s => s.Id);
        }

        //create a list of timeslots for a sitting category
        public async Task CreateSCTimeslots(TimeSpan startTime, TimeSpan duration, TimeSpan interval, int sittingCategoryId)
        {
            var tList = new List<SCTimeslot>();
            if (duration.TotalMinutes % interval.TotalMinutes == 0)
            {
                var tStartTime = startTime;
                var tEndTime = startTime.Add(interval);
                for (int i = 0; i < duration.TotalMinutes / interval.TotalMinutes; i++)
                {
                    tList.Add(new SCTimeslot(sittingCategoryId, tStartTime, tEndTime));
                    tStartTime = tStartTime.Add(interval);
                    tEndTime = tEndTime.Add(interval);
                }
                _cxt.SCTimeslots.AddRange(tList);
                await _cxt.SaveChangesAsync();
            }
            else
            {
                throw new Exception(message: "Duration must be divisible by interval");
            }
        }

        //create a list of tables for a sitting category
        public async Task CreateSCTables(int sittingCategoryId, int[] tablesId)
        {
            var tList = new List<SCTable>();
            foreach (var ti in tablesId)
            {
                tList.Add(new SCTable(sittingCategoryId, ti));
            }
            _cxt.SCTables.AddRange(tList);
            await _cxt.SaveChangesAsync();
        }

        //load all exisitng SCs
        public async Task<List<SittingCategory>> GetSCs()
        {
            return await _cxt.SittingCategories
                .Include(s => s.SCSittings)
                .Include(s => s.SCTables).ThenInclude(t => t.Table)
                .Include(s => s.SCTimeslots)
                .ToListAsync();
        }

        //find a SC by id
        public async Task<SittingCategory> GetSCById(int sittingCategoryId)
        {
            var allSC = await GetSCs();
            return allSC.FirstOrDefault(i => i.Id == sittingCategoryId);
        }

        //display table names in SC detail page
        public void GetTableStrings(SittingCategory sc)
        {
            var mainTables = sc.SCTables.FindAll(t => t.Table.Area == "Main").OrderBy(t => t.TableId);
            var outsideTables = sc.SCTables.FindAll(t => t.Table.Area == "Outside").OrderBy(t => t.TableId);
            var balconyTables = sc.SCTables.FindAll(t => t.Table.Area == "Balcony").OrderBy(t => t.TableId);
            string mainString = "", outsideString = "", balconyString = "";

            if (mainTables.Any()) { foreach (var t in mainTables) { mainString += t.Table.Name + " "; } }
            if (outsideTables.Any()) { foreach (var t in outsideTables) { outsideString += t.Table.Name + " "; } }
            if (balconyTables.Any()) { foreach (var t in balconyTables) { balconyString += t.Table.Name + " "; } }

            ViewBag.mainString = (mainString == "") ? "None" : mainString;
            ViewBag.outsideString = (outsideString == "") ? "None" : outsideString;
            ViewBag.balconyString = (balconyString == "") ? "None" : balconyString;
        }
        #endregion
    }
}
