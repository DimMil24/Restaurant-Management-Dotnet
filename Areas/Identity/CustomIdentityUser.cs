using Microsoft.AspNetCore.Identity;

namespace Restaurant_Manager.Areas.Identity
{
	public class CustomIdentityUser : IdentityUser
	{
		public string? Name { get; set; }
		public long? RestaurantId { get; set; }
	}
}
