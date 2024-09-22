using Microsoft.AspNetCore.Identity;

namespace GameForum.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Description {  get; set; }
        public DateTime BirthDay { get; set; }
        public string? Sex { get; set; }
        public List<Topic>? Topics { get; set; } = new List<Topic>();
        public List<Review>? Reviews { get; set; } = new List<Review>();
        public bool IsBanned { get; set; }
        public bool IsMuted { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
    }
}