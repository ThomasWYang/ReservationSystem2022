using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Models.SittingUnit
{
    public class Allocate
    {

        public Data.Reservation CurrentReservation { get; set; }
        public List<SCTimeslot> SCTimeslots { get; set; }
        public List<SCTable> SCTables { get; set; }
        public List<Data.SittingUnit> FullSittingUnits { get; set; }

        public List<int> SelectedSittingUnitsId { get; set; }


        //DTO

        public List<SittingUnitDTO> SittingUnitsDTO { get; set; } = new List<SittingUnitDTO>();
        public List<SCTableDTO> SCTablesDTO { get; set; } = new List<SCTableDTO>();
        public List<SCTimeslotDTO> SCTimeslotsDTO { get; set; } = new List<SCTimeslotDTO>();

        //For dropdown list demo
        public int[] SittingUnitsId { get; set; }
        public MultiSelectList SittingUnits { get; set; }


    }

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
