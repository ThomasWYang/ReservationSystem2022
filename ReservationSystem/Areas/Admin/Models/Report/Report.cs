using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystem.Data;

namespace ReservationSystem.Areas.Admin.Models.Report
{
    public class Report
    {
        public ReportSitting Sitting { get; set; }
        public ReportCustomer Customer { get; set; }
        public ReportReservation Reservation { get; set; }
        public class ReportSitting
        {
            
            [Display(Name = "Last 30 days Average Sitting Occupancy")]
            public string AvgOcpy { get; set; }

            [Display(Name = "Last 30 days Highest Sitting Occupancy")]
            public string HighestOcpy { get; set; }
        }
        public class ReportCustomer
        {
            [Display(Name = "Most Loyal Customer")]
            public string MostLoyalCustomer { get; set; }

            [Display(Name = "Registration Rate")]
            public string RegistrationRate { get; set; }
        }

        public class ReportReservation
        {
            [Display(Name = "Last 30 days Average Expected Dining Duration")]
            public string AverageDuation { get; set; }

            [Display(Name = "Last 30 days Most Popular Way of Booking")]
            public string WayOfBooking { get; set; }

            [Display(Name = "Last 30 days Average Customer per Booking")]
            public string NumOfGuests { get; set; }

            [Display(Name = "Last 30 days Reservation Cancel Rate")]
            public string CancelRate { get; set; }

            [Display(Name = "Last 30 days Reservation No-show Rate")]
            public string NoShowRate { get; set; }


        }
    }
}
