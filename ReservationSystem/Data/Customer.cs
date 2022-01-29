using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data.Users;

namespace ReservationSystem.Data
{
    public class Customer
    {
        public int Id { get; private set; }

        [Display(Name = "First Name")]
        public string CustFName { get; set; }

        [Display(Name = "Last Name")]
        public string CustLName { get; set; }

        [Display(Name = "Email")]
        public string CustEmail { get; set; }

        [Display(Name = "Phone")]
        public string CustPhone { get; set; }
        //public int MemberId { get; set; }

        #region RELATIONSHIPS        
        public List<Reservation> Reservations { get; private set; }
        public string IdentityUserId { get; set; }
        public virtual IdentityUser IdentityUser { get; set; }
        //public Member Member { get; set; }
        #endregion
    }
}
