using Microsoft.AspNetCore.Authorization;
using Restaurant_Manager.Models;
using Restaurant_Manager.Policies.Requirement;

namespace Restaurant_Manager.Policies.Handler;

public class CategoryAuthorizationHandler : AuthorizationHandler<RestaurantIdRequirement,Category>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RestaurantIdRequirement requirement,
        Category resource)
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