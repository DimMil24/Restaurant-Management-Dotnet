namespace Restaurant_Manager.Models.Requests;

public class NewProductRequest
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; }
}