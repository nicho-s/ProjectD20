using GameForum.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace GameForum.ViewModels
{
    public class UpdPasswordViewModel 
    {
        public string Id { get; set; } = string.Empty;

        //Не локалізовано!!!
        [Required(ErrorMessage = "Введіть поточний пароль")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть новий пароль")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{6,}$", 
            ErrorMessage = "Мінімальна довжина 6, в слові повинні бути щонайменше:  1 Вел. буква, 1 мал. буква, 1 спец. символ і 1 цифра")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Повторінь новий пароль")]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }

    }
}
