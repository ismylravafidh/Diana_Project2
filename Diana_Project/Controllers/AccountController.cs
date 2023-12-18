using Diana_Project.ViewModels.AccountVm;
using Microsoft.AspNetCore.Mvc;

namespace Diana_Project.Controllers
{
	[AutoValidateAntiforgeryToken]
	public class AccountController : Controller
	{
		UserManager<AppUser> _userManager;
		SignInManager<AppUser> _signInManager;

		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginVm loginVm)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			AppUser user = await _userManager.FindByNameAsync(loginVm.UsernameOrEmail);

			if (user == null)
			{
				user = await _userManager.FindByEmailAsync(loginVm.UsernameOrEmail);

				if(user == null)
				{
					ModelState.AddModelError("", "Username or Email or Password incorrect !!!");
					return View();
				}
			}

			var result = _signInManager.CheckPasswordSignInAsync(user, loginVm.Password, true).Result;

			if (result.IsLockedOut)
			{
				ModelState.AddModelError(String.Empty, "Zehmet olmasa biraz sonra yeniden cehd edin .");
				return View();
			}
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Username or Email or Password incorrect !!!");
				return View();
			}

			await _signInManager.SignInAsync(user, loginVm.RememberMe);

			return RedirectToAction("Index" , "Home");
		}
		public async Task<IActionResult> Register()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterVm registerVm)
		{
			if(!ModelState.IsValid)
			{
				return View();
			}
			AppUser user = new AppUser()
			{
				Name = registerVm.Name,
				Email = registerVm.Email,
				Surname = registerVm.Surname,
				UserName=registerVm.Username
			};
			var result = await _userManager.CreateAsync(user , registerVm.Password);

			if (!result.Succeeded)
			{
				foreach(var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View();
			}

			await _signInManager.SignInAsync(user, false);

			return RedirectToAction("Index", "Home");
		}
		public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
