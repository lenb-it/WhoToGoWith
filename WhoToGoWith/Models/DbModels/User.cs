using System.ComponentModel.DataAnnotations;

namespace WhoToGoWith.Models.DbModels
{
	public class User
	{
		[Required] public string Password { get; set; }

		[Required] public string UserName { get; set; }

		[Required] public string AboutMe { get; set; }

		public byte[] Avatar { get; set; }
	}
}
