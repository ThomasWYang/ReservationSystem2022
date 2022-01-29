using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Controllers
{
    public class HomeController : AdminAreaBaseController
    {
        public HomeController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager) : base(cxt, userManager)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
