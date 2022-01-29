using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Data
{
    public class Employee
    {
        public int Id { get; private set; }

        [Display(Name = "First Name")]
        public string EmpFName { get; set; }

        [Display(Name = "Last Name")]
        public string EmpLName { get; set; }

        [Display(Name = "Email")]
        public string EmpEmail { get; set; }

        [Display(Name = "Phone")]
        public string EmpPhone { get; set; }

        #region RELATIONSHIPS
        public string IdentityUserId { get; set; }
        public virtual IdentityUser IdentityUser { get; set; }

        #endregion
    }
}
