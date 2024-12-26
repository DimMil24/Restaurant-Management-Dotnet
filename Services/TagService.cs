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

    public async Task<List<Tag>?> GetAllTags()
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

    public void UpdateTagsToRestaurant(Restaurant? restaurant,List<long>? tags)
    {
        if (restaurant != null)
        {
            var currentTagIds = restaurant.Tags.Select(rt => rt.TagId).ToList();

            if (tags != null)
            {
                // Determine the tags to be added and removed
                var tagsToAdd = tags.Except(currentTagIds).ToList();

                // Add new tags
                foreach (var tagId in tagsToAdd)
                {
                    var rt = new RestaurantTag
                    {
                        RestaurantId = restaurant.Id,
                        TagId = tagId
                    };
                    _dbContext.RestaurantTag.Add(rt);
                }
            }
            List<long> tagsToRemove;
            tagsToRemove = tags == null ? currentTagIds : currentTagIds.Except(tags).ToList();
            foreach (var tagId in tagsToRemove)
            {
                var tagToRemove = restaurant.Tags.FirstOrDefault(rt => rt.TagId == tagId);
                if (tagToRemove != null)
                {
                    _dbContext.RestaurantTag.Remove(tagToRemove);
                }
            }
        }
    }

    public async Task<List<RestaurantTag>> GetTagsByRestaurantId(Guid restaurantId)
    {
        return await _dbContext.RestaurantTag.Where(r =>r.RestaurantId == restaurantId ).ToListAsync();
    }
}