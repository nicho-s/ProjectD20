using System.ComponentModel.DataAnnotations;

namespace GameForum.Models.DTO
{
    public class LoginModel
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
        public bool IsBanned {  get; set; }
    }
}
