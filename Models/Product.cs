using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant_Manager.Models
{
    public class Product
    {
        public long Id { get; set; }
        [MaxLength(100)]
        [Required (AllowEmptyStrings = false)]
        public string Name { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        
        public long CategoryId { get; set; }
        public bool Available { get; set; } = true;
        [MaxLength(300)]
		public string? Description { get; set; }
		public Guid RestaurantId { get; set; }
		
		public Category Category { get; set; }
		public Restaurant? Restaurant { get; set; }
	}
}
