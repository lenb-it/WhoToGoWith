using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WhoToGoWith.Models.ViewModels
{
	public class SettingsViewModel
	{
		public string Password { get; set; }

		public string ConfirmPassword { get; set; }

		public string AboutMe { get; set; }

		public IFormFile Avatar { get; set; }
	}
}
