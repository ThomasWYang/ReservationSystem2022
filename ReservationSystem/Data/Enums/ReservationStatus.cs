using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Data.Enums
{
    public enum ReservationStatus
    {
        //New ,
        Pending = 0,
        Confirmed = 1,
        //Modified ,
        Seated = 2,
        Completed = 3,

        Cancelled = 4,
        Expired = 5,
    }

}
