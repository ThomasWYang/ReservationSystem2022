using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Models.SittingCategory
{
    public class CreateSC : IValidatableObject
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Please enter 2 to 50 characters")]
        [RegularExpression(".*[^ ].*", ErrorMessage = "Cannot contain blank space only")]
        [Remote("IsNameAvailble", "SittingCategory")]
        public string Name { get; set; }

        [Required]
        [Range(1, 100)]
        public int Capacity { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression("^([0-9]|0[0-9]|1[0-9]|2[0-3]):([0-5][0-9])(:[0-5][0-9])?$", ErrorMessage = "Please enter time from 00:00 - 23:59(seconds optional)")]
        public TimeSpan StartTime { get; set; }


        public TimeSpan Duration { get => EndTime - StartTime; }

        [Required]
        [Display(Name = "End Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression("^([0-9]|0[0-9]|1[0-9]|2[0-3]):([0-5][0-9])(:[0-5][0-9])?$", ErrorMessage = "Please enter time from 00:00 - 23:59(seconds optional)")]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Display(Name = "Timeslot (hours)")]
        [Range(0, 24)]
        public int IntervalHours { get; set; }

        [Required]
        [Display(Name = "Timeslot (minutes)")]
        [Range(0, 59)]
        public int IntervalMinutes { get; set; }

        [Required]
        [Display(Name = "Tables")]
        public int[] TablesId { get; set; }
        public MultiSelectList Tables { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime >= EndTime) { yield return new ValidationResult("EndTime must be greater than StartTime", new List<string> { "StartTime" , "EndTime" }); }
            if (IntervalHours == 0 && IntervalMinutes == 0)
            {
                yield return new ValidationResult("Timeslot duration cannot be zero", new[] { "IntervalHours" , "IntervalMinutes" });
            }
            else
            {
                var Interval = new TimeSpan(IntervalHours, IntervalMinutes, 0);
                if (Duration.TotalMinutes % Interval.TotalMinutes != 0)
                {
                    yield return new ValidationResult("Sitting category duration must be divisible by the timeslot duration", new[] { "EndTime", "IntervalHours", "IntervalMinutes" });
                }
            }
            
        }


    }
}
