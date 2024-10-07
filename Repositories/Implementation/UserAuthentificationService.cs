using GameForum.Models;
using GameForum.Models.DTO;
using GameForum.Repositories.Abstract;
using GameForum.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GameForum.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthentificationService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private ILogger<UserAuthenticationService> logger;

        public UserAuthenticationService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ILogger<UserAuthenticationService> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }
        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Invalid email";
                return status;
            }
            if (user.IsBanned)
            {
                status.StatusCode = 0;
                status.StatusMessage = "User has been blocked";
                return status;
            }
            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusCode = 0;
                status.StatusMessage = "Invalid Password";
                return status;
            }

            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);
            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                status.StatusCode = 1;
                status.StatusMessage = "Logged in successfully";
                logger.LogInformation($"User {user.Email} successfully logged in.");
            }
            else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.StatusMessage = "User is locked out";
            }
            else
            {
                status.StatusCode = 0;
                status.StatusMessage = "Error on logging in";
            }

            return status;
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<Status> RegistrAsync(RegistrModel model)
        {
            var status = new Status();

            // Перевірка наявності користувача з таким же іменем
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Користувач з таким іменем вже існує";
                return status;
            }

            // Створення нового користувача
            ApplicationUser user = new ApplicationUser()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = model.Name,
                Email = model.Email,
                UserName = model.Username,
                Sex = model.Sex,
                BirthDay = model.BirthDay,
                EmailConfirmed = true,
                IsBanned = false,
                IsMuted = false,
                FailedLoginAttempts = 0,
                LockoutEnd = null,
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.StatusMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return status;
            }

            if (!await roleManager.RoleExistsAsync(model.Role))
            {
                await roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            if (await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user, model.Role);
            }

            status.StatusCode = 1;
            status.StatusMessage = "Ви успішно зареєструвалися";
            return status;
        }


        public async Task<Status> EditUserAsync(string userId, UserViewModel model)
        {
            var status = new Status();

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "User not found";
                return status;
            }

            user.Id = model.Id; 
            user.Name = model.Name;
            user.UserName = model.UserName;
            user.ProfilePicture = model.ProfilePicture;
            user.Description = model.Description;
            user.Sex = model.Sex;
            user.BirthDay = model.BirthDay;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.StatusMessage = "User update failed";
                return status;
            }

            status.StatusCode = 1;
            status.StatusMessage = "User updated successfully";
            return status;
        }
    }
}