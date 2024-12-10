using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;
using Restaurant_Manager.Models.Requests;

namespace Restaurant_Manager.Controllers
{
	[Route("[controller]/v1")]
	[ApiController]
	public class ApiController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public ApiController(ApplicationDbContext applicationDbContext) 
		{ 
			_context = applicationDbContext;
		}

		[HttpGet]
		[Route("product/{restaurantId}/{productId}")]
		public async Task<IActionResult> GetProduct(long restaurantId, long productId)
		{
			var Product = await _context.Product.FirstOrDefaultAsync(u => u.Id == productId && u.RestaurantId == restaurantId);
			return Product == null ? NotFound() : Ok(Product);
		}

		[HttpPost]
		[Route("newOrder")]
		public async Task<IActionResult> NewOrder([FromBody] NewOrderRequest newOrderRequest)
		{
			if (ModelState.IsValid)
			{
				using var transaction = _context.Database.BeginTransaction();
				//var restaurant_id = long.Parse(User.FindFirst("RestaurantId")?.Value);
				CustomerOrder order = new CustomerOrder() { UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) };
				var restaurant = await _context.Restaurant.FindAsync(newOrderRequest.RestaurantId);
				order.Restaurant = restaurant;
				var prods = new List<OrderProduct>();
				for (int i = 0; i < newOrderRequest.ItemQuantity.Count; i++)
				{
					var prod = await _context.Product.FindAsync(newOrderRequest.ItemQuantity[i].Id);
					if (prod != null) {
						var quantity = newOrderRequest.ItemQuantity[i].Quantity;
						prods.Add(new OrderProduct { Quantity = quantity, Price = prod.Price * quantity, Product = prod, CustomerOrder = order });
					}
				}
				order.OrderProducts = prods;
				_context.Add(order);
				await _context.SaveChangesAsync();
				transaction.Commit();
				return Ok();
			}
			return BadRequest();
		}
	}
}
