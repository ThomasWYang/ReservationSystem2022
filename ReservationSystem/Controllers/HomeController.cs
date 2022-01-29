using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservationSystem.Data;
using ReservationSystem.Models;
using ReservationSystem.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        #region DI
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _cxt;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext cxt, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _cxt = cxt;
            _userManager = userManager;
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> IndexMember()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var custAuthenticated = await _cxt.Customers.FirstOrDefaultAsync(c => c.IdentityUserId == user.Id);
            ViewData["Name"] = custAuthenticated.CustFName + " " + custAuthenticated.CustLName;
            return View();
        }

        public IActionResult ThankYouPage()
        {
            return View();
        }

        [Authorize]
        public IActionResult RedirectUser()
        {
            if (User.IsInRole("Admin") || User.IsInRole("Employee"))
            {
                return RedirectToAction("Index","Home", new { area = "Admin" });
            }
            else if (User.IsInRole("Member"))
            {
                return RedirectToAction("IndexMember", "Home", new { area = "" });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
