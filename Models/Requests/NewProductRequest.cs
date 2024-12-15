using Microsoft.AspNetCore.Mvc;

namespace Restaurant_Manager.Models.Requests;

public class NewProductRequest
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    [ModelBinder(Name = "Category.Name")]
    public string Category { get; set; }
}