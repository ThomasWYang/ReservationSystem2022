using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Data.Enums;
using ReservationSystem.Services;

namespace ReservationSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [EnableCors("MyPolicy")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        #region DI
        private readonly ApplicationDbContext _cxt;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomerService _customerService;
        public ReservationsController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager, CustomerService customerService)
        {
            _cxt = cxt;
            _userManager = userManager;
            _customerService = customerService;
        }
        #endregion


        #region methods

        // GET: api/reservations
        [HttpGet("")]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _cxt.Reservations
                .Include(r => r.Sitting.SittingCategory)
                .Include(r => r.Customer)
                .OrderBy(r => r.Sitting.Date)
                .ToListAsync();

            //var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve};
            //var rJson = JsonSerializer.Serialize(r, options);

            var reservationsDTO = new List<ReservationDTO>();
            foreach (var reservation in reservations)
            {
                reservationsDTO.Add(new ReservationDTO
                {
                    Id = reservation.Id,
                    SittingDate = reservation.Sitting.Date.ToShortDateString(),
                    Arrival = reservation.ExpectedStartTime.ToString(),
                    Leave = reservation.ExpectedEndTime.ToString(),
                    Guests = reservation.NumOfGuests,
                    Status = reservation.Status.ToString(),
                    Customer = reservation.Customer.CustEmail,
                    WayOfBooking = reservation.WayOfBooking,
                    SittingCategoryName = reservation.Sitting.SittingCategory.Name,
                    Notes = reservation.Notes
                });
            }
            return Ok(reservationsDTO);
        }


        // GET: api/reservations/customer/aa@a.com
        [HttpGet("customer/{custEmail}")]
        public async Task<ActionResult<Reservation>> GetReservationsByEmail(string custEmail)
        {
            var customer = await _cxt.Customers.FirstOrDefaultAsync(c => c.CustEmail == custEmail);
            if (customer == null)
            {
                return NotFound();
            }

            var reservations = await _cxt.Reservations
                .Where(r => r.Customer == customer)
                .Include(r => r.Sitting.SittingCategory)
                .Include(r => r.Customer)
                .OrderBy(r => r.Sitting.Date)
                .ToListAsync();

            var reservationsDTO = new List<ReservationDTO>();
            foreach (var reservation in reservations)
            {
                reservationsDTO.Add(new ReservationDTO
                {
                    Id = reservation.Id,
                    SittingDate = reservation.Sitting.Date.ToShortDateString(),
                    Arrival = reservation.ExpectedStartTime.ToString(),
                    Leave = reservation.ExpectedEndTime.ToString(),
                    Guests = reservation.NumOfGuests,
                    Status = reservation.Status.ToString(),
                    Customer = reservation.Customer.CustEmail,
                    WayOfBooking = reservation.WayOfBooking,
                    SittingCategoryName = reservation.Sitting.SittingCategory.Name,
                    Notes = reservation.Notes
                });
            }
            return Ok(reservationsDTO);
        }


        #endregion

        public class ReservationDTO
        {
            public int Id { get; set; }
            public int Guests { get; set; }
            public string Arrival { get; set; }
            public string Leave { get; set; }
            public string Notes { get; set; }
            public string WayOfBooking { get; set; }
            public string Status { get; set; }
            public string Customer { get; set; }
            public string SittingDate { get; set; }
            public string SittingCategoryName { get; set; }
        }
    }
}
