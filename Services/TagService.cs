using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;

namespace Restaurant_Manager.Services;

public class TagService
{
    private readonly ApplicationDbContext _dbContext;
    
    public TagService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tag?> FindTagByIdAsync(long id)
    {
        return await _dbContext.Tag.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Tag>> GetAllTags()
    {
        return await _dbContext.Tag.OrderBy(t => t.Name).AsNoTracking().ToListAsync();
    }

    public async Task AddTagsToRestaurant(long[] tagId, Guid restaurantId)
    {
        var tags = new List<RestaurantTag>();
        foreach (var tag in tagId)
        {
            var tagEntity = await FindTagByIdAsync(tag);
            RestaurantTag restaurantTag = new RestaurantTag()
            {
                RestaurantId = restaurantId,
                TagId = tag,
            };
            tags.Add(restaurantTag);
        }
        
        await _dbContext.RestaurantTag.AddRangeAsync(tags);
        await _dbContext.SaveChangesAsync();
    }
}