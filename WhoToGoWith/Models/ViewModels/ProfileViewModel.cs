using System.Collections.Generic;
using WhoToGoWith.Models.DbModels;

namespace WhoToGoWith.Models.ViewModels
{
	public class ProfileViewModel
	{
		public List<Event> Events { get; set; }

		public List<int> CountReadyToEvent { get; set; }

		public User User { get; set; }
	}
}
