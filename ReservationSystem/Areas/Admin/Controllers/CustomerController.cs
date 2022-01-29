using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Controllers
{
    public class CustomerController : AdminAreaBaseController
    {
        public CustomerController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager) : base(cxt, userManager)
        {
        }
        public async Task<IActionResult> IndexCustomer()
        {
            var customers = await _cxt.Customers.ToListAsync();
            return View(customers);
        }
    }
}
