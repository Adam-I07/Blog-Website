using AspNetCoreHero.ToastNotification.Abstractions;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notification;
        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, INotyfService notyfService) {
            _userManager = userManager;
            _signInManager = signInManager;
            _notification = notyfService;

        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet("Login")]
        public IActionResult Login() {
            if(!HttpContext.User.Identity.IsAuthenticated) {
                return View(new LoginVM());
            }
            return RedirectToAction("Index", "User", new {area = "Admin"});
            
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVM vm) {
            if (!ModelState.IsValid) {
                return View(vm);
            }

            var verifyUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == vm.Username);
            if (verifyUser == null) {
                _notification.Error("Username is invalid");
                return View(vm);
            }

            var verifyPassword = await _userManager.CheckPasswordAsync(verifyUser, vm.Password);
            if (!verifyPassword) {
                _notification.Error("Password is invalid");
                return View(vm);
            }

            await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, true);
            _notification.Success("Login Successful");
            return RedirectToAction("Index", "User", new {area="Admin"});
        }

        [HttpPost]
        public IActionResult Logout() {
            _signInManager.SignOutAsync();
            _notification.Success("Logged out!");
            return RedirectToAction("Index", "Home", new {area = ""});
        }
    }
}
 