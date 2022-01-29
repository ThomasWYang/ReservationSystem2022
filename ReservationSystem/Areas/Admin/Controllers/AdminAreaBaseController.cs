using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Data;

namespace ReservationSystem.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin, Employee")]
    public abstract class AdminAreaBaseController : Controller
    {
        protected readonly ApplicationDbContext _cxt;
        protected readonly UserManager<IdentityUser> _userManager;

        public AdminAreaBaseController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager)
        {
            _cxt = cxt;
            _userManager = userManager;
        }
        
    }
}
