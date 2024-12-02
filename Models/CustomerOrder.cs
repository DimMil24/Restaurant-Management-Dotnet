using System.ComponentModel.DataAnnotations;

namespace Restaurant_Manager.Models
{
    public class CustomerOrder
    {
        public long Id { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } = DateTimeOffset.Now.UtcDateTime;
        public bool Open { get; set; } = true;
        public long RestaurantId { get; set; }


        public List<OrderProduct>? OrderProducts { get; set; }
        public Restaurant? Restaurant { get; set; }

    }
}
