namespace WhoToGoWith.Models.DbModels
{
	public class ReadyForEvent
	{
		public string UserName { get; set; }

		public User User { get; set; }

		public int EventId { get; set; }

		public Event Event { get; set; }
	}
}
