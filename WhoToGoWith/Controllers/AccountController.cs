using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhoToGoWith.Contexts;
using WhoToGoWith.Models.DbModels;
using WhoToGoWith.Models.ViewModels;
using WhoToGoWith.Services;

namespace WhoToGoWith.Controllers
{
    public class AccountController : Controller
    {
        public AccountController(WhoToGoWithContext dbContext, IUserService userService, IEventService eventService)
	    {
            _dbContext = dbContext;
            _userService = userService;
            _eventService = eventService;
	    }

        private readonly WhoToGoWithContext _dbContext;
        private readonly IUserService _userService;
        private readonly IEventService _eventService;

        [HttpGet] public IActionResult Profile(string id)
	    {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var user = _userService.GetUserWithEmptyPassword(id);

            if (user is null) return NotFound();

            var events = _dbContext.Events.Where(e => e.Date > DateTime.Now &&
                                                    e.UserName == id).ToList();
            events = events.Where(e => e.MaxCountPeople == 0 ||
                                    e.MaxCountPeople > _eventService.GetCountReadyUser(e.Id)).ToList();

            var countReadyToEvent = new List<int>();

            foreach (var iEvent in events) countReadyToEvent.Add(_eventService.GetCountReadyUser(iEvent.Id));

            var model = new ProfileViewModel()
            {
                User = user,
                Events = events,
                CountReadyToEvent = countReadyToEvent,
            };

		    return View(model);
	    }

        [HttpGet, Authorize] public IActionResult Settings()
        {
            return View();
        }

	    [HttpPost, Authorize] public IActionResult Settings(SettingsViewModel model)
	    {
		    if (!string.IsNullOrWhiteSpace(model.Password) &&
			    !string.IsNullOrWhiteSpace(model.ConfirmPassword))
		    {
			    if (!model.Password.Equals(model.ConfirmPassword, StringComparison.Ordinal))
			    {
				    ModelState.AddModelError("", "Пароли не совпадают!");
			    }
		    }

		    if (model.Avatar is not null)
		    {
                if (model.Avatar.ContentType != "image/jpeg" && 
                    model.Avatar.ContentType != "image/png")
			    {
                    ModelState.AddModelError("", "Проверьте формат файла!");
                }            
            }

		    if (ModelState.ErrorCount > 0) return View(model);

            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (!string.IsNullOrWhiteSpace(model.Password) &&
                !string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                if (model.Password.Equals(model.ConfirmPassword, StringComparison.Ordinal))
                {
                    user.Password = model.Password;
                }
            }

            if (!string.IsNullOrWhiteSpace(model.AboutMe)) user.AboutMe = model.AboutMe;

            if (model.Avatar is not null)
		    {
                using var br = new BinaryReader(model.Avatar.OpenReadStream());

                user.Avatar = br.ReadBytes((int)model.Avatar.Length);
            }

            _dbContext.SaveChanges();

            return RedirectToAction("Profile", "Account", new { id = User.Identity.Name });
        }

	    [HttpGet] public IActionResult Register()
	    {
            if (User.Identity.IsAuthenticated) return NotFound();

            return View();
	    }

        [HttpPost] public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.ConfirmPassword != model.Password)
		    {
                ModelState.AddModelError("", "Пароли не совпадают");
                return View(model);
		    }

            if (_userService.IsUserExists(model.UserName))
		    {
                ModelState.AddModelError("", "Такой пользователь уже зарегистрирован!");
                return View(model);
            }

            _dbContext.Users.Add(new User
            {
                UserName = model.UserName,
                Password = model.Password,
                AboutMe = $"Привет, меня зовут { model.UserName }.",
            });

            _dbContext.SaveChanges();

            await Authenticate(model.UserName);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet] public IActionResult Login()
	    {
            if (User.Identity.IsAuthenticated) return NotFound();

		    return View();
	    }

        [HttpPost] public async Task<IActionResult> Login(LoginViewModel model)
	    {
            if (ModelState.IsValid)
            {
                var user = _userService.GetUser(model.UserName, model.Password);

                if (user is not null)
                {
                    await Authenticate(model.UserName);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Проверьте введеные данные!");
            }

            return View(model);
	    }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };

            var id = new ClaimsIdentity(claims,
                                        "ApplicationCookie",
                                        ClaimsIdentity.DefaultNameClaimType,
                                        ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                            new ClaimsPrincipal(id));
        }
    }
}
