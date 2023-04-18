using System.ComponentModel.DataAnnotations;

namespace WhoToGoWith.Models.ViewModels
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "Введите логин!")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Введите пароль!"), DataType(DataType.Password)] 
		public string Password { get; set; }
	}
}
