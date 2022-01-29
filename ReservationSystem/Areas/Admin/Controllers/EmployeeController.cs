using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Services;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : AdminAreaBaseController
    {
        public EmployeeController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager) : base(cxt, userManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> IndexEmployee()
        {
            var employees = await _cxt.Employees.ToListAsync();
            return View(employees);
        }

        [HttpGet]
        public IActionResult CreateEmployee()
        {
            var m = new Models.Employee.CreateEmployee();
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(Models.Employee.CreateEmployee m)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = m.Email, Email = m.Email, PhoneNumber = m.Phone };
                var result = await _userManager.CreateAsync(user, m.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Employee");
                    var e = new Data.Employee
                    {
                        EmpFName = m.FName,
                        EmpLName = m.LName,
                        EmpEmail = m.Email,
                        EmpPhone = m.Phone,
                        IdentityUserId = user.Id
                    };
                    
                    _cxt.Employees.Add(e);

                    await _cxt.SaveChangesAsync();

                    return RedirectToAction("IndexEmployee", "Employee", new { area = "Admin" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(m);
        }
    }
}
