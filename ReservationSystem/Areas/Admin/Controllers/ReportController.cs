using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Areas.Admin.Models.Report;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Controllers
{
    public class ReportController : AdminAreaBaseController
    {
        public ReportController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager) : base(cxt, userManager)
        {
        }

        public async Task<IActionResult> IndexReport()
        {
            bool check = _cxt.Reservations.Any();
            ViewData["Check"] = check;

            if (!check)
            {
                return View();
            }
            else
            {
                var m = new Report();
                await ReportSitting(m);
                await ReportCustomer(m);
                await ReportReservation(m);
                return View(m);
            }

        }


        #region REPORT METHODS

        public async Task<Report> ReportSitting(Report model)
        {
            var today = DateTime.Today;
            var lastMonth = DateTime.Today.AddDays(-30);
            var sittings = await _cxt.Sittings.Include(s => s.SittingCategory).Include(s => s.Reservations)
                .Where(s => s.Date >= lastMonth && s.Date < today).ToListAsync();

            var highestOccupancy = sittings.Max(s => s.Occupancy);

            var totalUsedCapacity = sittings.Sum(s => s.UsedCapacity);
            var totalCapacity = sittings.Sum(s => s.Capacity);


            var avgOccupancy = (decimal)totalUsedCapacity / totalCapacity;

            model.Sitting = new Report.ReportSitting
            {
                AvgOcpy = avgOccupancy.ToString("P"),
                HighestOcpy = highestOccupancy.ToString("P"),
            };

            return model;
        }

        public async Task<Report> ReportCustomer(Report model)
        {
            var customers = await _cxt.Customers.Include(c => c.Reservations).ToListAsync();
            var loyalCustomer = customers.OrderByDescending(c => c.Reservations.Count(r => r.Status == Data.Enums.ReservationStatus.Completed)).FirstOrDefault();
            var customerNum = customers.Count;
            var memberNum = customers.Count(c => c.IdentityUserId != null);
            var registrationRate = (decimal)memberNum / customerNum;
            model.Customer = new Report.ReportCustomer
            {
                MostLoyalCustomer = $"{loyalCustomer.CustFName} {loyalCustomer.CustLName}",
                RegistrationRate = registrationRate.ToString("P"),
            };
            return model;
        }
        private async Task<Report> ReportReservation(Report model)
        {
            var today = DateTime.Today;
            var lastMonth = DateTime.Today.AddDays(-30);
            var reservations = await _cxt.Reservations.Where(r => r.Sitting.Date >= lastMonth && r.Sitting.Date < today).ToListAsync();
            var totalReservation = reservations.Count;
            var canceledReservation = reservations.Count(r => r.Status == Data.Enums.ReservationStatus.Cancelled);
            var expiredReservation = reservations.Count(r => r.Status == Data.Enums.ReservationStatus.Expired);
            var expectedDuration = reservations.Average(r => r.ExpectedEndTime.Subtract(r.ExpectedStartTime).TotalHours);
            var popularWay = reservations.GroupBy(r => r.WayOfBooking).Select(group => new
            {
                WayofBooking = group.Key,
                Count = group.Count(),
            }).OrderByDescending(g => g.Count).FirstOrDefault().WayofBooking;
            var numOfGuests = reservations.Average(r => r.NumOfGuests);
            var cancelRate = (decimal)canceledReservation / totalReservation;
            var noShowRate = (decimal)expiredReservation / totalReservation;
            model.Reservation = new Report.ReportReservation
            {
                AverageDuation = $"{expectedDuration:f2} Hours",
                WayOfBooking = popularWay,
                NumOfGuests = $"{numOfGuests:f2} customers",
                CancelRate = cancelRate.ToString("P"),
                NoShowRate = noShowRate.ToString("P"),
            };
            return model;
        }



        #endregion


    }
}
