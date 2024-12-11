using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;
using Restaurant_Manager.Models.Requests;
using Restaurant_Manager.Services;

namespace Restaurant_Manager.Controllers
{
	[Route("[controller]/v1")]
	[ApiController]
	public class ApiController : ControllerBase
	{
		private readonly OrderService _orderService;
		private readonly ProductService _productService;
		public ApiController(OrderService orderService, ProductService productService)
		{
			_orderService = orderService;
			_productService = productService;
		}

		[HttpGet]
		[Route("product/{restaurantId}/{productId}")]
		public async Task<IActionResult> GetProduct(long restaurantId, long productId)
		{
			var product = await _productService.FindProductByIdAndRestaurantId(restaurantId, productId);
			return product == null ? NotFound() : Ok(product);
		}

		[HttpPost]
		[Route("newOrder")]
		public async Task<IActionResult> NewOrder(NewOrderRequest newOrderRequest)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId != null) await _orderService.NewOrder(newOrderRequest, userId);
			return Ok();
		}
	}
}
