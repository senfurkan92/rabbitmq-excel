using DAL.Context;
using DAL.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WEB.Controllers
{
	public class UserController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;

		public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

        [HttpGet]
        public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
        public IActionResult Login(LoginDto dto)
        {
			if (ModelState.IsValid)
			{
				var appUser = _userManager.FindByEmailAsync(dto.Email).Result;
				if (appUser != null)
				{
					if (_userManager.CheckPasswordAsync(appUser, dto.Password).Result)
					{
						_signInManager.PasswordSignInAsync(appUser, dto.Password, dto.RememberMe, false).Wait();
						return RedirectToAction("Index", "Home");
					}
				}
            }

            ModelState.AddModelError("", "invalid user");

            return View(dto);
        }
    }
}
