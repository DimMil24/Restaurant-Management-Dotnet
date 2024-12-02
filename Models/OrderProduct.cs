using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant_Manager.Models
{
    public class OrderProduct
    {
        public long Id { get; set; }
        public long Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public long ProductId { get; set; }
        public long CustomerOrderId { get; set; }

        public CustomerOrder CustomerOrder { get; set; }
        public Product Product { get; set; }
    }
}
