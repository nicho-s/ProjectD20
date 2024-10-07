using GameForum.Models;
using GameForum.Repositories.Abstract;
using GameForum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;

namespace Lab4_5.Controllers
{
    public class ForumController : Controller
    {
        private readonly ForumDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserAuthentificationService _service;
        private readonly ILogger<ForumController> _logger;

        public ForumController(ForumDBContext context, IUserAuthentificationService service, UserManager<ApplicationUser> userManager, ILogger<ForumController> logger)
        {
            _context = context;
            _userManager = userManager;
            _service = service;
            _logger = logger;
        }

        public IActionResult Main()
        {
            return View();
        }

        public async Task<IActionResult> Account(Guid? id)
        {
            // Завантаження користувача з бази даних
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.ToString());

            // Перевірка, чи користувач знайдено
            if (user == null)
            {
                return NotFound();
            }

            // Створення моделі для відображення
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                ProfilePicture = user.ProfilePicture,
                Description = user.Description,
                BirthDay = user.BirthDay,
                Sex = user.Sex,
                UserName = user.UserName,
                IsBanned = user.IsBanned,
                IsMuted = user.IsMuted,
            };

            // Розрахунок віку
            if (user.BirthDay != null)
            {
                userViewModel.Age = CalculateAge(user.BirthDay);
            }

            // Передача моделі у view
            return View(userViewModel);
        }

        private int CalculateAge(DateTime birthDay)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDay.Year;

            // Перевірка, чи вже відзначений день народження у цьому році
            if (birthDay.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            var model = new UpdUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                ProfilePicture = user.ProfilePicture,
                Description = user.Description,
                BirthDay = user.BirthDay,
                Sex = user.Sex,
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(UpdUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (user == null)
            {
                return NotFound();
            }

            // Оновити властивості користувача з даних моделі
            user.Name = model.Name;
            user.UserName = model.UserName;
            user.ProfilePicture = model.ProfilePicture;
            user.Description = model.Description;
            user.BirthDay = model.BirthDay;
            user.Sex = model.Sex;

            // Оновити користувача в базі даних
            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            var model = new UpdPasswordViewModel
            {
                Id = user.Id
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id, UpdPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            // Перевіряємо поточний пароль користувача
            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!passwordCheck)
            {
                ModelState.AddModelError(string.Empty, "Неправильний поточний пароль.");
                return View(model);
            }

            // Змінюємо пароль
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["Message"] = "Пароль успішно змінено";
                return RedirectToAction("Edit", new { id = user.Id });
            }

            // У разі помилок додаємо їх до моделі
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Ban(BanUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.id);
            if (user == null) return NotFound();

            if (!user.IsBanned) user.IsBanned = true;
            
            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnBan(BanUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model); 

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.id);
            if (user == null) return NotFound(); 

            if (user.IsBanned)  user.IsBanned = false;  

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Mute(MuteUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.id);
            if (user == null) return NotFound();

            if (!user.IsMuted) user.IsMuted = true;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnMute(MuteUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.id);
            if (user == null) return NotFound();

            if (user.IsMuted) user.IsMuted = false;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return View(model);
        }

        [HttpPost]
        public IActionResult SetCulture(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) }
            );
            return LocalRedirect(returnUrl);
        }
    }
}