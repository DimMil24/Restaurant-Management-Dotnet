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

		public OrderService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<CustomerOrder> FindOrder(long? id)
		{
			return await _context.CustomerOrder
						.Include(c => c.OrderProducts)
						.ThenInclude(p => p.Product)
						.FirstOrDefaultAsync(m => m.Id == id);
		}
		
		public async Task<CustomerOrder?> FindOrderNoDetails(long? id) => await _context.CustomerOrder
											.FirstOrDefaultAsync(m => m.Id == id);

		public async Task<List<CustomerOrder>> GetRestaurantOrders(long? restaurantId)
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
				var prod = await _context.Product.FindAsync(newOrderRequest.ItemQuantity[i].Id);
				if (prod != null) {
					var quantity = newOrderRequest.ItemQuantity[i].Quantity;
					prods.Add(new OrderProduct { Quantity = quantity, Price = prod.Price * quantity, Product = prod, CustomerOrder = order });
				}
			}
			order.OrderProducts = prods;
			_context.Add(order);
			await _context.SaveChangesAsync();
			await transaction.CommitAsync();
		}

		public async Task CompleteOrder(CustomerOrder customerOrder)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();
			customerOrder.Open = false;
			_context.Update(customerOrder);
			await _context.SaveChangesAsync();
			await transaction.CommitAsync();
		}

		public async Task DeleteOrder(CustomerOrder customerOrder)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();
			_context.CustomerOrder.Remove(customerOrder);
			await _context.SaveChangesAsync();
			await transaction.CommitAsync();
		}
	}
}
