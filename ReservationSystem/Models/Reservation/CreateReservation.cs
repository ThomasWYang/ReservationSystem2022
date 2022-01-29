using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystem.Data;
using ReservationSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Models.Reservation
{
    public class CreateReservation
    {        
        public string MaxDate { get; set; }
        public string MinDate { get; set; }        
        public int CustomerId { get; set; }
        public CustomerDTO Customer { get; set; }

        #region RESERVATION INFO

        [Required, Display(Name = "Choose a date:")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime SelectedDate { get; set; }

        [Required, Display(Name = "Choose a session:")]
        public int SelectedSittingId { get; set; }

        [Required, Display(Name = "Expected Arrival")]
        public string ExpectedStartTime { get; set; }

        [Required, Display(Name = "Expected Leave")]
        public string ExpectedEndTime { get; set; }

        [Required, Display(Name = "Number of Guests")]
        public int NumOfGuests { get; set; }

        public string Notes { get; set; }

        #endregion
    }

    public class SittingDTO
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string SCName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class CustomerDTO
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
    }
}
