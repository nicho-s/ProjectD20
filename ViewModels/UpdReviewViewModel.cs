using System.ComponentModel.DataAnnotations;

namespace GameForum.ViewModels
{
    public class UpdReviewViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Заповніть обов'язкове поле")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Допустима довжина від 10 до 2000 символів")]
        public string Text { get; set; }
    }
}
