using Restaurant_Manager.Areas.Identity;

namespace Restaurant_Manager.Models
{
	public class Restaurant
	{
		public Guid Id { get; set; }
		public required string Name { get; set; }
		public bool IsOpen { get; set; } = true;
		public string? Description { get; set; }
		
		public List<Category>? Categories { get; set; }
	}
}
