using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;
using Restaurant_Manager.Models.Requests;

namespace Restaurant_Manager.Services;

public class RestaurantService
{
    private readonly ApplicationDbContext _context;
    private readonly TagService _tagService;

    public RestaurantService(ApplicationDbContext context, TagService tagService)
    {
        _context = context;
        _tagService = tagService;
    }

    public async Task<List<Restaurant>> GetAllRestaurants()
    {
        return await _context.Restaurant.ToListAsync();
    }

    public async Task<Restaurant?> FindRestaurantById(Guid? id)
    {
        return await _context.Restaurant.FirstOrDefaultAsync(m => m.Id == id);
    }
    
    public async Task<Restaurant?> FindRestaurantByIdWithTags(Guid? id)
    {
        return await _context.Restaurant
            .Include(r => r.Tags)
            .ThenInclude(t => t.Tag)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task DeleteRestaurant(Restaurant restaurant)
    {
        _context.Restaurant.Remove(restaurant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRestaurant(Restaurant restaurant,EditRestaurantRequest editRestaurantRequest)
    {
        restaurant.Name = editRestaurantRequest.Name;
        restaurant.Description = editRestaurantRequest.Description;
        restaurant.IsOpen = editRestaurantRequest.IsOpen;
        _context.Update(restaurant);
        _tagService.UpdateTagsToRestaurant(restaurant, editRestaurantRequest.TagList);
        await _context.SaveChangesAsync();
    }

    public bool RestaurantExists(Guid id)
    {
        return _context.Restaurant.Any(e => e.Id == id);
    }
}