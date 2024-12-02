using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant_Manager.Models
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "text")]
        public ProductCategory Category { get; set; }
        public bool Available { get; set; } = true;
		public string? Description { get; set; }
		public long RestaurantId { get; set; }
		public Restaurant? Restaurant { get; set; }
	}
}
