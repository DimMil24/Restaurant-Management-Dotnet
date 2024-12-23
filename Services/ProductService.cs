using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;
using Restaurant_Manager.Models.Requests;

namespace Restaurant_Manager.Services
{
	public class ProductService
	{
		private readonly ApplicationDbContext _context;
		private readonly CategoryService _categoryService;

		public ProductService(ApplicationDbContext context, CategoryService categoryService)
		{
			_context = context;
			_categoryService = categoryService;
		}

		public async Task<Product?> FindProduct(long? id) => await _context.Product.Include(c => c.Category).FirstOrDefaultAsync(p => p.Id == id);

		public async Task<Product?> FindProductByIdAndRestaurantId(Guid? restaurantId, long? productId)
			=> await _context.Product.FirstOrDefaultAsync(u => u.Id == productId && u.RestaurantId == restaurantId);

		public async Task CreateProduct(NewProductRequest productRequest, Guid restaurantId)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();
			Category? category = (await _categoryService.GetCategoryByNameAndByRestaurantAsync(restaurantId, productRequest.Category));
			Product newProduct;
			if (category == null)
			{
				Category newCategory = await _categoryService.CreateCategoryAsync(restaurantId, productRequest.Category);
				newProduct = new Product()
				{
					Name = productRequest.Name,
					Price = productRequest.Price,
					Description = productRequest.Description,
					RestaurantId = restaurantId,
					Category = newCategory
				};
			}
			else
			{
				newProduct = new Product()
				{
					Name = productRequest.Name,
					Price = productRequest.Price,
					Description = productRequest.Description,
					RestaurantId = restaurantId,
					Category = category
				};
			}
			_context.Add(newProduct);
			await _context.SaveChangesAsync();
			await transaction.CommitAsync();
		}

		public async Task UpdateProduct(Product product)
		{
			_context.Update(product);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteProduct(Product product)
		{
			_context.Product.Remove(product);
			await _context.SaveChangesAsync();
		}

		public async Task<List<Product>> GetRestaurantProducts(Guid? restaurantId)
		{
			return await _context.Product
				.Include(c => c.Category)
				.Where(e => e.RestaurantId == restaurantId)
				.OrderBy(e => e.Category.Id)
				.ToListAsync();
		}

		public bool ProductExists(long id)
		{

			return  _context.Product.Any(e => e.Id == id);
		}

		public bool ProductExistsByNameAndRestaurant(Guid restaurantId, string productRequestName)
		{
			return  _context.Product.Any(e => e.Name == productRequestName && e.RestaurantId == restaurantId);
		}
	}
}
