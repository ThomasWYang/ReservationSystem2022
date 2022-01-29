using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystem.Data.Enums;

namespace ReservationSystem.Data
{
    public class SCTimeslot
    {
        public int Id { get; private set; }
        public int SittingCategoryId { get; private set; }
        public TimeSpan StartTime { get; private set; }

        public TimeSpan Duration { get => EndTime - StartTime; }
        public TimeSpan EndTime { get; private set; }
        
        #region RELATIONSHIPS        
        public SittingCategory SittingCategory { get; private set; }
        #endregion

        #region CONSTRUCTOR
        public SCTimeslot(int sittingCategoryId, TimeSpan startTime, TimeSpan endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
            SittingCategoryId = sittingCategoryId;
        }
        #endregion

    }
}
