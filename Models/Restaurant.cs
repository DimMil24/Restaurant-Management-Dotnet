namespace Restaurant_Manager.Models
{
	public class Restaurant
	{
		public long Id { get; set; }
		public required string Name { get; set; }
		public bool IsOpen { get; set; } = true;
		public string? Description { get; set; }
	}
}
