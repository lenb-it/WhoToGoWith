using WhoToGoWith.Models.DbModels;

namespace WhoToGoWith.Services
{
	public interface IUserService
	{
		public User GetUser(string userName, string password);

		public User GetUserWithEmptyPassword(string userName);

		public bool IsUserExists(string userName); 
	}
}
