using Microsoft.AspNetCore.Authorization;
using Restaurant_Manager.Models;
using Restaurant_Manager.Policies.Requirement;

namespace Restaurant_Manager.Policies.Handler
{
	public class OrderAuthorizationHandler : AuthorizationHandler<RestaurantIdRequirement, CustomerOrder>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RestaurantIdRequirement requirement, CustomerOrder resource)
		{
			var restaurantIdClaim = context.User.FindFirst("RestaurantId");
			if (restaurantIdClaim == null)
			{
				return Task.CompletedTask;
			}

			if (long.Parse(restaurantIdClaim.Value) == resource.RestaurantId)
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
