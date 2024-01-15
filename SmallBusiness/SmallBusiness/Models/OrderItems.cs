using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.Models
{
    public class OrderItems
    {
        [Key]
        public int OrderItemsId { get; set; }
        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int? ProductId { get; set; }

        public Product Product { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int ProductQuantity { get; set; }
    }
}
