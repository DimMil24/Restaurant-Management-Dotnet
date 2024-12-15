using Microsoft.Build.Framework;

namespace Restaurant_Manager.Models;

public class Category
{
    public long Id { get; set; }
    [Required]
    public string Name { get; set; }
    public Guid RestaurantId { get; set; }
    
    public Restaurant? Restaurant { get; set; }
}