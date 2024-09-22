using GameForum.Models;
using GameForum.Models.DTO;
using GameForum.Repositories.Abstract;
using GameForum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameForum.Controllers
{
    public class UserAuthController : Controller
    {
        private readonly ForumDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserAuthentificationService _service;

        public UserAuthController(ForumDBContext context, IUserAuthentificationService service, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _service = service;
        }

        //Registration
        public IActionResult Registr()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registr(RegistrModel model)
        {
            model.Role = "user";
            model.IsBanned = false;
            model.IsMuted = false;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    var result = await _service.RegistrAsync(model);
                    TempData["msg"] = result.StatusMessage;

                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError("", "Email зайнятий");
                }

            }
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View(model);
        }

        //Login
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    var result = await _service.LoginAsync(model);
        //    if (result.StatusCode == 1)
        //    {
        //        return RedirectToAction("Main", "Forum");
        //    }
        //    else
        //    {
        //        TempData["msg"] = result.StatusMessage;
        //        return RedirectToAction(nameof(Login));
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.Username);

            if (user != null && user.LockoutEnd > DateTime.UtcNow)
            {
                // Користувач заблокований, показуємо повідомлення
                TempData["msg"] = "Ваш обліковий запис заблокований. Спробуйте пізніше.";
                return View(model);
            }

            var result = await _service.LoginAsync(model);

            if (result.StatusCode == 1)
            {
                user.FailedLoginAttempts = 0; // Скидаємо лічильник невдалих спроб
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Main", "Forum");
            }
            else
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 3)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(5); // Блокувати на 5 хвилин
                    TempData["msg"] = "Ви заблоковані на 5 хвилин через занадто багато невдалих спроб.";
                }
                else
                {
                    TempData["msg"] = result.StatusMessage;
                }

                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Login));
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync();
            return RedirectToAction("Main", "Forum");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserList()
        {
            var users = await _userManager.Users.ToListAsync();

            if (users.Count == 0)
            {
                ViewBag.Message = "На разі на сайті ніхто ще не реєструвався";
            }

            var model = users.Select(user => new ListUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UserListSearch(string searchString)
        {
            var users = await _userManager.Users.ToListAsync();

            // Фільтруємо користувачів на основі пошукового запиту
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u =>
                    u.UserName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    u.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                // Якщо немає результатів пошуку
                if (users.Count == 0)
                {
                    ViewBag.Message = "Такого користувача немає";
                }
            }

            var model = users.Select(user => new ListUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name
            }).ToList();

            return View("UserList", model);  // Повертаємо ту саму сторінку зі списком користувачів
        }
    }
}
