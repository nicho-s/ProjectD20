using System.ComponentModel.DataAnnotations;

namespace GameForum.Models.DTO
{
    public class RegistrModel
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Мінімальна довжина 6, в слові має бути:  1 Вел. буква, 1 мал. буква, 1 спец. символ і 1 цифра")]
        public string? Password { get; set; }
        
        [Required]
        [Compare("Password")]
        public string? PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Будь ласка виберіть дату народження")]
        [DataType(DataType.Date)]
        public DateTime BirthDay { get; set; }


        [Required(ErrorMessage = "Виберіть стать")]
        public string Sex { get; set; }

        public string? Role { get; set; }
    }
}
