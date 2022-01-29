using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Data
{
    public class SittingCategory
    {
        public int Id { get; set; }
        public string Name { get; private set; }
        public int Capacity { get; private set; }

        [Display(Name = "Start")]
        public TimeSpan StartTime { get; private set; }
        public TimeSpan Duration { get; private set; }

        [Display(Name = "End")]
        public TimeSpan EndTime { get => StartTime + Duration; }

        #region RELATIONSHIPS        
        public List<SCTimeslot> SCTimeslots { get; private set; }
        public List<SCTable> SCTables { get; private set; }
        public List<Sitting> SCSittings { get; private set; }
        #endregion
    }
}
