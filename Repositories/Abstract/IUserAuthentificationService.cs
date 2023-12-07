using GameForum.Models.DTO;
using Lab4_5.ViewModels;

namespace GameForum.Repositories.Abstract
{
    public interface IUserAuthentificationService
    {
        Task<Status> LoginAsync(LoginModel model);
        Task<Status> RegistrAsync(RegistrModel model);
        Task LogoutAsync();
        Task<Status> EditUserAsync(string userId, UserViewModel model);
    }
}
