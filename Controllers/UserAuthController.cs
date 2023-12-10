using GameForum.Models;
using GameForum.Models.DTO;
using GameForum.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _service.LoginAsync(model);
            if (result.StatusCode == 1)
            {
                return RedirectToAction("Main", "Forum");
            }
            else
            {
                TempData["msg"] = result.StatusMessage;
                return RedirectToAction(nameof(Login));
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync();
            return RedirectToAction("Main", "Forum");
        }

    }
}
