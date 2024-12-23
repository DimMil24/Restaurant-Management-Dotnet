namespace Restaurant_Manager.Models;

public class RestaurantTag
{
    public long Id { get; set; }
    public Guid RestaurantId { get; set; }
    public long TagId { get; set; }
    public Tag Tag { get; set; }
    public Restaurant Restaurant { get; set; }
}