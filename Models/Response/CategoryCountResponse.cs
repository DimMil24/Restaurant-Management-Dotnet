namespace Restaurant_Manager.Models.Response;

public class CategoryCountResponse
{
    public long Id { get; set; }
    public string CategoryName { get; set; }
    public long ProductCount { get; set; }
}