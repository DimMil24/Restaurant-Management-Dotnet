using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;

namespace Restaurant_Manager.Services;

public class CategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Category.Include(r => r.Restaurant).ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(long id)
    {
        return await _context.Category.Include(r => r.Restaurant).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> GetCategoryByNameAndByRestaurantAsync(Guid restaurantId,string name)
    {
        return await _context.Category.Include(r => r.Restaurant)
            .FirstOrDefaultAsync(c => c.Name == name && c.Restaurant!.Id == restaurantId );
    }

    public async Task<Category> CreateCategoryAsync(Guid restaurantId,string category)
    {
        Category newCategory = new Category()
        {
            Name = category,
            RestaurantId = restaurantId
        };
        _context.Add(newCategory);
        await _context.SaveChangesAsync();
        return newCategory;
    }

    public async Task UpdateCategoryAsync(Category? category,string name)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        category!.Name = name;
        _context.Update(category);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task DeleteCategoryAsync(Category category)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        _context.Category.Remove(category);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    
    public bool CategoryExists(long id)
    {
        return _context.Category.Any(e => e.Id == id);
    }

    public async Task<List<Category>> GetCategoriesByRestaurant(Guid restaurantId)
    {
        return await _context.Category.Include(r => r.Restaurant).Where(r => r.RestaurantId == restaurantId).ToListAsync();
    }
}