using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallBusiness.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

       

        [Required]
        [Range(0.1, double.MaxValue)] 
        public decimal TotalPrice { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public ICollection<OrderItems> OrderItems { get; set; }


    }
}
