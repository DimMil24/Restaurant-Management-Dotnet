using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Restaurant_Manager.Models.Requests;

public class NewProductRequest
{
    [Required]
    public string Name { get; set; }
    public decimal Price { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
    [ModelBinder(Name = "Category.Name")]
    public string Category { get; set; }
}