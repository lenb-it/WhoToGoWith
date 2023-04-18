using System;
using System.Linq;
using WhoToGoWith.Contexts;
using WhoToGoWith.Models.DbModels;

namespace WhoToGoWith.Services
{
	public class UserService : IUserService
	{
		public UserService(WhoToGoWithContext dbContext)
		{
			_dbContext = dbContext;
		}

		private readonly WhoToGoWithContext _dbContext;

		public User GetUser(string userName, string password)
		{
			var users = _dbContext.Users.ToList();

			foreach(var user in users)
			{
				if (userName.Equals(user.UserName, StringComparison.Ordinal) && 
					password.Equals(user.Password, StringComparison.Ordinal)) return user;
			}

			return null;
		}

		public User GetUserWithEmptyPassword(string userName)
		{
			var user = _dbContext.Users.FirstOrDefault(u => u.UserName == userName);

			if (user is null) return null;

			user.Password = string.Empty;

			return user;
		}

		public bool IsUserExists(string userName)
		{
			var user = _dbContext.Users.FirstOrDefault(u => u.UserName == userName);

			return user is not null;
		}
	}
}
