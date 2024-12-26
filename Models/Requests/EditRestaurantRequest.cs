using System.ComponentModel.DataAnnotations;

namespace Restaurant_Manager.Models.Requests;

public class EditRestaurantRequest
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
    public bool IsOpen { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
    public List<long>? TagList { get; set; }
}