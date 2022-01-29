using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystem.Data;
using ReservationSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Models.Sitting
{
    public class CreateSitting
    {
        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Sitting Category")]
        public int SittingCategoryId { get; set; }
        //public SelectList SittingCategories { get; set; }
        public Data.SittingCategory SittingCategory { get; set; }        
    }
    public class ScDTO
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
