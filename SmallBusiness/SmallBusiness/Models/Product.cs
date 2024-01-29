using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallBusiness.Models
{
    public class Product 
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product Description is required")]
        [StringLength(255, ErrorMessage = "Product description cannot exceed 255 characters")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Product price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Product price must be greater than 0")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "Product quantity in stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Product quantity in stock must be greater than or equal to 0")]
        public int ProductQuantityStock { get; set; }

        [Range(0, 100, ErrorMessage = "Product sale percentage must be between 0 and 100")]
        public decimal ProductSale { get; set; }


        [Range(0, 5, ErrorMessage = "Review rate must be between 0 and 5")]
        public int ReviewRate { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateAt { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        [Required]
        public int ProfileId { get; set; }
        public Profile profile { get; set; }

        public List<Review> Reviews { get; set; } // Add this property to hold the reviews for the product

        public bool IsDelete { get; set; }

    }
}
