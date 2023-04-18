using System;
using System.ComponentModel.DataAnnotations;

namespace WhoToGoWith.Models.ViewModels
{
	public class AddEventViewModel
	{
		[DataType(DataType.DateTime), Required(ErrorMessage = "Введите дату!")] public DateTime Date { get; set; }

		[Required(ErrorMessage = "Введите название!")] public string Title { get; set; }

		[Required(ErrorMessage = "Введите информацию о событии!")] public string Information { get; set; }

		[Required(ErrorMessage = "Введите город!")] public string City { get; set; }

		public int? MaxCountPeople { get; set; }

		public string UserName { get; set; }
	}
}
