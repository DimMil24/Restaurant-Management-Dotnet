using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;

namespace Restaurant_Manager.Services;

public class RestaurantService
{
    private readonly ApplicationDbContext _context;

    public RestaurantService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Restaurant>> GetAllRestaurants()
    {
        return await _context.Restaurant.ToListAsync();
    }

    public async Task<Restaurant?> FindRestaurantById(Guid? id)
    {
        return await _context.Restaurant.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task DeleteRestaurant(Restaurant restaurant)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        _context.Restaurant.Remove(restaurant);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task UpdateRestaurant(Restaurant restaurant)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        _context.Update(restaurant);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public bool RestaurantExists(Guid id)
    {
        return _context.Restaurant.Any(e => e.Id == id);
    }
}