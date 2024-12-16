using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant_Manager.Services;

namespace Restaurant_Manager.Controllers
{
	[Authorize(Roles = "Owner,Admin")]
	public class OrderController : Controller
	{
		private readonly OrderService _orderService;
		private readonly IAuthorizationService _authorizationService;


		public OrderController(OrderService orderService, IAuthorizationService authorizationService)
		{
			_orderService = orderService;
			_authorizationService = authorizationService;
		}
		public async Task<IActionResult> Index()
		{
			var value = User.FindFirst("RestaurantId")?.Value;
			if (value == null) return View();
			
			var restaurantId = Guid.Parse(value);
			return View(await _orderService.GetRestaurantOrders(restaurantId));
		}

		public async Task<IActionResult> Details(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var customerOrder = await _orderService.FindOrder(id);
			
			if (customerOrder == null)
			{
				return NotFound();
			}

			var authorizationResult = await _authorizationService
		  .AuthorizeAsync(User, customerOrder, "RestaurantPolicy");

			if (authorizationResult.Succeeded)
			{
				return View(customerOrder);
			}
			else if (User.Identity!.IsAuthenticated)
			{
				return new ForbidResult();
			}
			else
			{
				return new ChallengeResult();
			}
		}
		
		public async Task<IActionResult> Delete(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var customerOrder = await _orderService.FindOrderNoDetails(id);
			if (customerOrder == null)
			{
				return NotFound();
			}

			var authorizationResult = await _authorizationService
		  .AuthorizeAsync(User, customerOrder, "RestaurantPolicy");

			if (authorizationResult.Succeeded)
			{
				return View(customerOrder);
			}
			else if (User.Identity!.IsAuthenticated)
			{
				return new ForbidResult();
			}
			else
			{
				return new ChallengeResult();
			}
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(long id)
		{
			var customerOrder = await _orderService.FindOrderNoDetails(id);
			if (customerOrder == null)
			{
				return NotFound();
			}
			await _orderService.DeleteOrder(customerOrder);
			return RedirectToAction(nameof(Index));
		}
		

		[HttpPost]
		public async Task<IActionResult> Complete(long? id)
		{
			var customerOrder = await _orderService.FindOrderNoDetails(id);
			if (customerOrder is null)
			{
				return NotFound();
			}
			if (!customerOrder.Open)
			{
				return BadRequest();
			} 
			else
			{
				await _orderService.CompleteOrder(customerOrder);
				return RedirectToAction(nameof(Index));
			}
		}
	}
}
