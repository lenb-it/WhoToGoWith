using System.ComponentModel.DataAnnotations;

namespace WhoToGoWith.Models.ViewModels
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "Введите логин!")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Введите пароль!"), DataType(DataType.Password)]
		public string Password { get; set; }

		[Required(ErrorMessage = "Подтвердите пароль!"), DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }
	}
}
