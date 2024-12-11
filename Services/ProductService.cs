using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;

namespace Restaurant_Manager.Services
{
	public class ProductService
	{
		private readonly ApplicationDbContext _context;

		public ProductService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Product> FindProduct(long? id) => await _context.Product.FindAsync(id);

		public async Task CreateProduct(Product product, long restaurantId)
		{
			using var transaction = _context.Database.BeginTransaction();
			product.RestaurantId = restaurantId;
			_context.Add(product);
			await _context.SaveChangesAsync();
			transaction.Commit();
		}

		public async Task UpdateProduct(Product product)
		{
			using var transaction = _context.Database.BeginTransaction();
			_context.Update(product);
			await _context.SaveChangesAsync();
			transaction.Commit();
		}

		public async Task DeleteProduct(Product product)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();
			_context.Product.Remove(product);
			await _context.SaveChangesAsync();
			await transaction.CommitAsync();
		}

		public async Task<List<Product>> GetRestaurantProducts(long restaurantId)
		{
			return await _context.Product.Where(e => e.RestaurantId == restaurantId).ToListAsync();
		}

		public bool ProductExists(long id)
		{

			return  _context.Product.Any(e => e.Id == id);
		}
	}
}
