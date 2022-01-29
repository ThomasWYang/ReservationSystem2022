using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Models.Sitting
{
    public class DetailsSitting
    {


        public Data.Sitting Sitting { get; set; }

        public List<SittingUnitDTO> SittingUnitsDTO { get; set; } = new List<SittingUnitDTO>();
        public List<SCTableDTO> SCTablesDTO { get; set; } = new List<SCTableDTO>();
        public List<SCTimeslotDTO> SCTimeslotsDTO { get; set; } = new List<SCTimeslotDTO>();

        public class SittingUnitDTO
        {

            public int Id { get; set; }
            public int TimeslotId { get; set; }
            public int TableId { get; set; }
            public int? ReservationId { get; set; }

        }


        public class SCTableDTO
        {

            public int Id { get; set; }
            public string Name { get; set; }
            public string Area { get; set; }

        }

        public class SCTimeslotDTO
        {

            public int Id { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }

        }
    }
}
