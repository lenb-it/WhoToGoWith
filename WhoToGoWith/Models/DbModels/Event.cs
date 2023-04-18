using System;
using System.ComponentModel.DataAnnotations;

namespace WhoToGoWith.Models.DbModels
{
	public class Event
	{
		public long Id { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime Date { get; set; }

		public string Title { get; set; }

		public string Information { get; set; }

		public int MaxCountPeople { get; set; }

		public string City { get; set; }

		public string UserName { get; set; }

		public User User { get; set; }
	}
}
