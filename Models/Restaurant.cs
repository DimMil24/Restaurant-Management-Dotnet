using System.ComponentModel.DataAnnotations;
using Restaurant_Manager.Areas.Identity;

namespace Restaurant_Manager.Models
{
	public class Restaurant
	{
		public Guid Id { get; set; }
		public required string Name { get; set; }
		public bool IsOpen { get; set; } = true;
		[MaxLength(500)]
		public string? Description { get; set; }
		
		public List<Category>? Categories { get; set; }
		
		public List<RestaurantTag> Tags { get; set; }
	}
}
