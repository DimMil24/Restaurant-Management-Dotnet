using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;

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
