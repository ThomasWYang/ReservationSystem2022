using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystem.Data.Enums;

namespace ReservationSystem.Data
{
    public class Table
    {
        public int Id { get; private set; }
        public string Area { get; private set; }
        public string Name { get; private set; }

        public Table(string area, string name)
        {
            Area = area;
            Name = name;
        }
    }
}
