using GameForum.Models.DTO;
using GameForum.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace GameForum.Controllers
{
    public class UserAuthController : Controller
    {
        private readonly IUserAuthentificationService _service;
        public UserAuthController(IUserAuthentificationService service)
        {
            this._service = service;
        }

        //Registration
        public IActionResult Registr()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registr(RegistrModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Role = "user";
            var result = await _service.RegistrAsync(model);
            TempData["msg"] = result.StatusMessage;
            return RedirectToAction(nameof(Registr));
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
