using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category Name is required")]
        public string CategoryName { get; set; }
        
        [Required]
        public string Image { get; set; }

        public List<Product> Products { get; set; }


    }
}
