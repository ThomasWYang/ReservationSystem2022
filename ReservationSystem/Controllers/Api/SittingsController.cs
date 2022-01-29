using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [EnableCors("MyPolicy")]
    [ApiController]
    public class SittingsController : ControllerBase
    {
        #region DI
        private readonly ApplicationDbContext _cxt;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomerService _customerService;
        public SittingsController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager, CustomerService customerService)
        {
            _cxt = cxt;
            _userManager = userManager;
            _customerService = customerService;
        }

        public UserManager<IdentityUser> UserManager => _userManager;
        #endregion

        #region methods

        // GET: api/sittings
        [HttpGet("")]
        public async Task<IActionResult> GetOpenSittings()
        {
            var sittings = await _cxt.Sittings.Include(s => s.SittingCategory).Include(s => s.Reservations).Where(s => s.Status == Data.Enums.SittingStatus.Open && s.Date >= DateTime.Today).OrderBy(s => s.Date).ToListAsync();
            if (sittings.Count == 0)
            {
                return NotFound();
            }
            var sittingsDTO = new List<SittingDTO>();
            foreach (var sitting in sittings)
            {
                sittingsDTO.Add(new SittingDTO
                {
                    Id = sitting.Id,
                    Date = sitting.Date.ToShortDateString(),
                    Status = sitting.Status.ToString(),
                    Category = sitting.SittingCategory.Name,
                    Capacity = sitting.Capacity,
                    Used = sitting.UsedCapacity,
                    Remaining = sitting.RemainingCapacity,
                    StartTime = sitting.SittingCategory.StartTime.ToString(),
                    EndTime = sitting.SittingCategory.EndTime.ToString(),
                });
            }

            return Ok(sittingsDTO);
        }

        // GET: api/sittings/1
        [HttpGet("{sittingId}")]
        public async Task<ActionResult<Reservation>> GetSittingById(int sittingId)
        {
            var sitting = await _cxt.Sittings.Include(s => s.SittingCategory).Include(s => s.Reservations).FirstOrDefaultAsync(s => s.Id == sittingId);
            if (sitting == null)
            {
                return NotFound();
            }
            var sittingDTO = new SittingDTO
            {
                Id = sitting.Id,
                Date = sitting.Date.ToShortDateString(),
                Status = sitting.Status.ToString(),
                Category = sitting.SittingCategory.Name,
                Capacity = sitting.Capacity,
                Used = sitting.UsedCapacity,
                Remaining = sitting.RemainingCapacity,
                StartTime = sitting.SittingCategory.StartTime.ToString(),
                EndTime = sitting.SittingCategory.EndTime.ToString(),
            };

            return Ok(sittingDTO);

        }

        // POST: api/sittings/1/newreservation
        [HttpPost("{sittingId}/newreservation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<Reservation>> CreateReservationBySittingId(int sittingId, ReservationDTO data)
        {

            var sitting = await _cxt.Sittings.Include(s => s.SittingCategory).Include(s => s.Reservations).FirstOrDefaultAsync(s => s.Id == sittingId);

            if (sitting == null || sitting.Status != Data.Enums.SittingStatus.Open || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var reservation = new Reservation
            {
                SittingId = sittingId,
                ExpectedStartTime = TimeSpan.Parse(data.ExpectedStartTime),
                ExpectedEndTime = TimeSpan.Parse(data.ExpectedEndTime),
                NumOfGuests = data.NumOfGuests,
                Notes = string.IsNullOrWhiteSpace(data.Notes) ? "N/A" : data.Notes.Trim(),
                TimeOfBooking = DateTime.Now,
                WayOfBooking = "App",
                Status = Data.Enums.ReservationStatus.Pending,
            };
            var custEntered = new Data.Customer
            {
                CustFName = data.CustFName,
                CustLName = data.CustLName,
                CustEmail = data.CustEmail,
                CustPhone = data.CustPhone
            };
            reservation.Customer = await _customerService.UpsertCustomerAsync(custEntered, true, false);



            _cxt.Reservations.Add(reservation);
            await _cxt.SaveChangesAsync();

            if (sitting.RemainingCapacity <= 0) { sitting.Status = Data.Enums.SittingStatus.Closed; }

            await _cxt.SaveChangesAsync();

            return Created(nameof(CreateReservationBySittingId), data);

        }

        //Testing data
        /*
         {
            "expectedStartTime":"8:00:00",
            "expectedEndTime":"10:00:00",
            "numOfGuests":2,
            "custFName":"ddd",
            "custLName":"ddd",
            "custEmail":"ddd@a.com",
            "custPhone":"11111111"
        }
        */

    #endregion
    public class SittingDTO
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public int Capacity { get; set; }
        public int Used { get; set; }
        public int Remaining { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class ReservationDTO
    {
        [Required]
        [Display(Name = "First Name")]
        [RegularExpression(".*[^ ].*")]
        public string CustFName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [RegularExpression(".*[^ ].*")]
        public string CustLName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Invalid email address")]
        public string CustEmail { get; set; }

        [Required]
        [Display(Name = "Phone")]
        [StringLength(10)]
        [DataType(DataType.PhoneNumber)]
        public string CustPhone { get; set; }


        [Required, Display(Name = "Expected Arrival")]
        public string ExpectedStartTime { get; set; }

        [Required, Display(Name = "Expected Leave")]
        public string ExpectedEndTime { get; set; }

        [Required, Display(Name = "Number of Guests")]
        public int NumOfGuests { get; set; }
        public string Notes { get; set; }

    }
}
}
