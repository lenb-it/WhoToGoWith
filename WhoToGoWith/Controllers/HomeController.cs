using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WhoToGoWith.Contexts;
using WhoToGoWith.Models;

namespace WhoToGoWith.Controllers
{
	public class HomeController : Controller
	{
		//TODO: сделать главную страницу
		public HomeController(WhoToGoWithContext dbContext)
		{
			_dbContext = dbContext;
		}

		private readonly WhoToGoWithContext _dbContext;

		public IActionResult Index()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
