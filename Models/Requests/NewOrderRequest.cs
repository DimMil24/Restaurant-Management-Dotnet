namespace Restaurant_Manager.Models.Requests
{
	public class NewOrderRequest
	{
		public long RestaurantId {  get; set; }
		public required List<ItemQuantityRequest> ItemQuantity { get; set; }
	}
}
