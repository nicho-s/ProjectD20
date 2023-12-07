using System.ComponentModel.DataAnnotations;

namespace Lab4_5.ViewModels
{
    public class UpdUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Поле не повинно пустувати")]
        [StringLength(100, ErrorMessage = "Ім'я не повинно перевищувати 100 символів")]
        public string? Name { get; set; }


        [Required(ErrorMessage = "Поле не повинно пустувати")]
        [StringLength(100, ErrorMessage = "Ім'я користувача не повинно перевищувати 100 символів")]
        public string? UserName { get; set; }

        [Range(1, 9, ErrorMessage = "Цифра повинна бути між 1 та 9")]
        public string? ProfilePicture { get; set; }


        [Required(ErrorMessage = "Поле не повинно пустувати")]
        [StringLength(500, ErrorMessage = "Опис не повинен перевищувати 500 символів")]
        public string? Description { get; set; }


        [Display(Name = "День народження")]
        public DateTime BirthDay { get; set; }

        [Display(Name = "Formatted BirthDay")]
        public string FormattedBirthDay => BirthDay.ToShortDateString();

        [Required(ErrorMessage = "Поле не повинно пустувати")]
        public string Sex { get; set; }
    }
}
