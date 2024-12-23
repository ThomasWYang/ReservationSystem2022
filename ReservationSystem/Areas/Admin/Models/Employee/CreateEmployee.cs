﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Models.Employee
{
    public class CreateEmployee
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [RegularExpression(".*[^ ].*")]
        public string FName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [RegularExpression(".*[^ ].*")]
        public string LName { get; set; }

        [Required]
        [Display(Name = "Phone")]
        [StringLength(10)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}
