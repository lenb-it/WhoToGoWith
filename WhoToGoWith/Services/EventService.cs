using System.Linq;
using WhoToGoWith.Contexts;

namespace WhoToGoWith.Services
{
	public class EventService : IEventService
	{
		public EventService(WhoToGoWithContext dbContext)
		{
			_dbContext = dbContext;
		}

		private readonly WhoToGoWithContext _dbContext;

		public int GetCountReadyUser(long idEvent)
		{
			return _dbContext.ReadyForEvents.Where(e => e.EventId == idEvent).Count();
		}
	}
}
