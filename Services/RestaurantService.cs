using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;

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

    public async Task UpdateRestaurant(Restaurant restaurant,long[] tags)
    {
        if (restaurant.Tags != null)
        {
            var currentTagIds = restaurant.Tags.Select(rt => rt.TagId).ToList();

            // Determine the tags to be added and removed
            var tagsToAdd = tags.Except(currentTagIds).ToList();
            var tagsToRemove = currentTagIds.Except(tags).ToList();

            // Add new tags
            foreach (var tagId in tagsToAdd)
            {
                var rt = new RestaurantTag
                {
                    RestaurantId = restaurant.Id,
                    TagId = tagId
                };
                _context.RestaurantTag.Add(rt);
            }
        
            foreach (var tagId in tagsToRemove)
            {
                var tagToRemove = restaurant.Tags.FirstOrDefault(rt => rt.TagId == tagId);
                if (tagToRemove != null)
                {
                    _context.RestaurantTag.Remove(tagToRemove);
                }
            }
        }
        _context.Update(restaurant);
        await _context.SaveChangesAsync();
    }

    public bool RestaurantExists(Guid id)
    {
        return _context.Restaurant.Any(e => e.Id == id);
    }
}