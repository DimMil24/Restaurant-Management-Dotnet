using Microsoft.AspNetCore.Identity;
using Restaurant_Manager.Models;

namespace Restaurant_Manager.Areas.Identity
{
	public class CustomIdentityUser : IdentityUser
	{
		public long? RestaurantId { get; set; }

		public Restaurant? Restaurant { get; set; }
	}
}
