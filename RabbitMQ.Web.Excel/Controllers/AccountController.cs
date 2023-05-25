using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ.Web.Excel.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var hasUser = await _userManager.FindByEmailAsync(Email);
            if (hasUser == null)
            {
                return View();
            }
            var signInManager=await _signInManager.PasswordSignInAsync(hasUser, Password,true,false);

            if (!signInManager.Succeeded)
            {
                return View("Kullanıcı adı veya şifre hatalı...");
            }
            return RedirectToAction(nameof(HomeController.Index),"Home");
        }
    }
}
