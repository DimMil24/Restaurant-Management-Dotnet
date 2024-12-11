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

		public async Task<Product?> FindProduct(long? id) => await _context.Product.FindAsync(id);

		public async Task<Product?> FindProductByIdAndRestaurantId(long? restaurantId, long? productId)
			=> await _context.Product.FirstOrDefaultAsync(u => u.Id == productId && u.RestaurantId == restaurantId);

		public async Task CreateProduct(Product product, long restaurantId)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();
			product.RestaurantId = restaurantId;
			_context.Add(product);
			await _context.SaveChangesAsync();
			await transaction.CommitAsync();
		}

		public async Task UpdateProduct(Product product)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();
			_context.Update(product);
			await _context.SaveChangesAsync();
			await transaction.CommitAsync();
		}

		public async Task DeleteProduct(Product product)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();
			_context.Product.Remove(product);
			await _context.SaveChangesAsync();
			await transaction.CommitAsync();
		}

		public async Task<List<Product>> GetRestaurantProducts(long? restaurantId)
		{
			return await _context.Product.Where(e => e.RestaurantId == restaurantId).ToListAsync();
		}

		public bool ProductExists(long id)
		{

			return  _context.Product.Any(e => e.Id == id);
		}
	}
}
