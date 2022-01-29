using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Data
{
    public class SCTable
    {
        public int Id { get; private set; }
        public int SittingCategoryId { get; private set; }
        public int TableId { get; private set; }

        #region RELATIONSHIPS
        public SittingCategory SittingCategory { get; set; }
        public Table Table { get; private set; }
        #endregion

        #region CONSTRUCTOR
        public SCTable(int sittingCategoryId, int tableId)
        {            
            SittingCategoryId = sittingCategoryId;
            TableId = tableId;
        }
        #endregion
    }
}
