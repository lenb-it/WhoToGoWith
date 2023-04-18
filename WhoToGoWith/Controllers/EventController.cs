using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhoToGoWith.Contexts;
using WhoToGoWith.Models.DbModels;
using WhoToGoWith.Models.ViewModels;
using WhoToGoWith.Services;

namespace WhoToGoWith.Controllers
{
	public class EventController : Controller
	{
		public EventController(WhoToGoWithContext dbContext, IEventService eventService, IUserService userService)
		{
			_dbContext = dbContext;
			_eventService = eventService;
			//_userService = userService;
		}

		private readonly WhoToGoWithContext _dbContext;
		private readonly IEventService _eventService;
		//private readonly IUserService _userService;

		[HttpGet] public IActionResult Index()
		{
			var events = _dbContext.Events.Where(e => e.Date > DateTime.Now && 
												 e.UserName != User.Identity.Name).ToList();
			var ready = _dbContext.ReadyForEvents.Where(r => r.UserName == User.Identity.Name).ToList();

			events = events.Where(e => !ready.Any(r => r.EventId == e.Id)).ToList();
			events = events.Where(e => e.MaxCountPeople == 0 || 
								  e.MaxCountPeople > _eventService.GetCountReadyUser(e.Id)).ToList();

			var countReadyToEvent = new List<int>();

			foreach(var iEvent in events) countReadyToEvent.Add(_eventService.GetCountReadyUser(iEvent.Id));

			var model = new EventViewModel()
			{
				SearchCity = string.Empty,
				Events = events,
				CountReadyToEvent = countReadyToEvent,
			};

			return View(model);
		}

		[HttpPost] public IActionResult Index(EventViewModel model)
		{
			if (string.IsNullOrWhiteSpace(model.SearchCity))
			{
				model.Events = _dbContext.Events.Where(e => e.Date > DateTime.Now && 
													   e.UserName != User.Identity.Name).ToList();
				var ready = _dbContext.ReadyForEvents.Where(r => r.UserName == User.Identity.Name).ToList();
				model.Events = model.Events.Where(e => !ready.Any(r => r.EventId == e.Id)).ToList();
			}
			else
			{
				model.Events = _dbContext.Events
					 .Where(e => e.City == model.SearchCity && 
							e.Date > DateTime.Now && e.UserName != 
							User.Identity.Name).ToList();
				var ready = _dbContext.ReadyForEvents.Where(r => r.UserName == User.Identity.Name).ToList();

				model.Events = model.Events.Where(e => !ready.Any(r => r.EventId == e.Id)).ToList();
			}

			return View(model);
		}

		[HttpGet, Authorize] public IActionResult DeleteEvent(int? id)
		{
			if (id is null) return NotFound();

			var deleteEvent = _dbContext.Events.FirstOrDefault(e => e.Id == id && e.UserName == User.Identity.Name);

			if (deleteEvent is null) return NotFound();

			var deleteUsersFromEvent = _dbContext.ReadyForEvents.Where(e => e.EventId == id).ToList();

			_dbContext.ReadyForEvents.RemoveRange(deleteUsersFromEvent);
			_dbContext.Events.Remove(deleteEvent);
			_dbContext.SaveChanges();

			return RedirectToAction("Profile", "Account", new { id = User.Identity.Name });
		}

		[HttpGet, Authorize] public IActionResult Ready()
		{
			var myReadyEvents = _dbContext.ReadyForEvents.Where(e => e.UserName == User.Identity.Name);
			var events = _dbContext.Events.Where(e => myReadyEvents.Any(re => re.EventId == e.Id)).ToList();
			var countReadyToEvent = new List<int>();
			
			foreach (var iEvent in events) countReadyToEvent.Add(_eventService.GetCountReadyUser(iEvent.Id));

			var model = new LeaveTheEventViewModel
			{
				Events = events,
				CountReadyToEvent = countReadyToEvent,
			};

			return View(model);
		}

		[HttpGet, Authorize] public IActionResult LeaveTheEvent(int id)
		{
			var leaveEvent = _dbContext.ReadyForEvents
				.FirstOrDefault(e => e.EventId == id && e.UserName == User.Identity.Name);

			if (leaveEvent is not null)
			{
				_dbContext.ReadyForEvents.Remove(leaveEvent);
				_dbContext.SaveChanges();
			}

			return RedirectToAction("Ready", "Event");
		}

		[HttpGet, Authorize] public IActionResult AddEvent()
		{
			return View();
		}

		[HttpPost] public IActionResult AddEvent(AddEventViewModel model)
		{
			if (string.IsNullOrWhiteSpace(model.Title) ||
				string.IsNullOrWhiteSpace(model.Information)||
				string.IsNullOrWhiteSpace(model.City))
			{
				ModelState.AddModelError(string.Empty, "Проверьте введеные данные!");
				return View(model);
			}

			var now = DateTime.Now;
			DateTime nowPlusOneHour = new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, now.Minute, 0);
			DateTime nowPlusFiveYear = new DateTime(now.Year + 5, now.Month, now.Day, now.Hour, now.Minute, 0);

			if (model.Date.Date < nowPlusOneHour.Date || model.Date.Date > nowPlusFiveYear.Date)
			{
				ModelState.AddModelError(string.Empty, "Введеные корректную дату!");
				return View(model);
			}

			if (model.MaxCountPeople is null || model.MaxCountPeople <= 0) model.MaxCountPeople = 0;

			var addEvent = new Event
			{
				City = model.City,
				Date = model.Date,
				Information = model.Information,
				Title = model.Title,
				MaxCountPeople = (int)model.MaxCountPeople,
				UserName = User.Identity.Name,
			};

			_dbContext.Events.Add(addEvent);
			_dbContext.SaveChanges();

			return RedirectToAction("Index", "Event");
		}

		[HttpGet] public IActionResult Event()
		{
			return View();
		}

		[HttpGet, Authorize] public IActionResult ReadyToEvent(int? id)
		{
			if (id is null) return NotFound();

			_dbContext.ReadyForEvents.Add(new ReadyForEvent
			{
				UserName = User.Identity.Name,
				EventId = (int)id,
			});
			_dbContext.SaveChanges();

			return RedirectToAction("Index", "Event");
		}
	}
}
