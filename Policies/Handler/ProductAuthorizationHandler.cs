using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Restaurant_Manager.Areas.Identity;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;
using Restaurant_Manager.Policies.Requirement;

namespace Restaurant_Manager.Policies.Handler
{
	public class ProductAuthorizationHandler : AuthorizationHandler<RestaurantIdRequirement, Product>
	{
		

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RestaurantIdRequirement requirement, Product resource)
		{
			var restaurantIdClaim = context.User.FindFirst("RestaurantId");
			if (restaurantIdClaim == null)
			{
				return Task.CompletedTask;
			}
			
			if (Guid.Parse(restaurantIdClaim.Value) == resource.RestaurantId)
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}

