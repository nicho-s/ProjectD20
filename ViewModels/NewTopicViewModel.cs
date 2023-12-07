using System.ComponentModel.DataAnnotations;

namespace GameForum.ViewModels
{
    public class NewTopicViewModel
	{
		[Required(ErrorMessage = "Заповніть обов'язкове поле")]
		[StringLength(100, MinimumLength = 10, ErrorMessage = "Допустима довжина від 10 до 100 символів")]
		public string? Title { get; set; }

		[Required(ErrorMessage = "Заповніть обов'язкове поле")]
		[StringLength(2000, MinimumLength = 10, ErrorMessage = "Допустима довжина від 10 до 2000 символів")]
		public string? Description { get; set; }
		public DateTime? CreatinTime { get; set; }
	}
}
