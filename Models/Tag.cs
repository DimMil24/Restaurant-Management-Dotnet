namespace Restaurant_Manager.Models;

public class Tag
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public List<RestaurantTag>? Restaurants { get; set; }
    
}