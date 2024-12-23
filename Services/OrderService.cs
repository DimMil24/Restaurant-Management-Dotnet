using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;
using Restaurant_Manager.Models.Requests;

namespace Restaurant_Manager.Services
{
	public class OrderService
	{
		private readonly ApplicationDbContext _context;
		private readonly ProductService _productService;

		public OrderService(ApplicationDbContext context, ProductService productService)
		{
			_context = context;
			_productService = productService;
		}

		public async Task<CustomerOrder> FindOrder(long? id)
		{
			return (await _context.CustomerOrder
				.Include(c => c.OrderProducts)!
				.ThenInclude(p => p.Product)
				.FirstOrDefaultAsync(m => m.Id == id))!;
		}
		
		public async Task<CustomerOrder?> FindOrderNoDetails(long? id) => await _context.CustomerOrder
											.FirstOrDefaultAsync(m => m.Id == id);

		public async Task<List<CustomerOrder>> GetRestaurantOrders(Guid? restaurantId)
		{
			return await _context.CustomerOrder.Include(e => e.User).Where(e => e.RestaurantId == restaurantId).ToListAsync();
		}

		public async Task NewOrder(NewOrderRequest newOrderRequest,string userId)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();
			CustomerOrder order = new CustomerOrder() { UserId = userId };
			var restaurant = await _context.Restaurant.FindAsync(newOrderRequest.RestaurantId);
			order.Restaurant = restaurant;
			var prods = new List<OrderProduct>();
			for (int i = 0; i < newOrderRequest.ItemQuantity.Count; i++)
			{
				var prod = await _productService.FindProductByIdAndRestaurantId(restaurant.Id,newOrderRequest.ItemQuantity[i].Id);
				
				if (prod != null) {
					var quantity = newOrderRequest.ItemQuantity[i].Quantity;
					prods.Add(new OrderProduct { Quantity = quantity, Price = prod.Price * quantity, Product = prod, CustomerOrder = order });
				}
			}
			order.OrderProducts = prods;
			await _context.AddAsync(order);
			await _context.SaveChangesAsync();
			await transaction.CommitAsync();
		}

		public async Task CompleteOrder(CustomerOrder customerOrder)
		{
			customerOrder.Open = false;
			_context.Update(customerOrder);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteOrder(CustomerOrder customerOrder)
		{
			_context.CustomerOrder.Remove(customerOrder);
			await _context.SaveChangesAsync();
		}
	}
}
