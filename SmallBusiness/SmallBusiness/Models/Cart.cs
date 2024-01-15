using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallBusiness.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        public string? UserId { get; set; }

        public User User { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public int TotalQuantity { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<CartItems> CartItems { get; set; }


    }
}
