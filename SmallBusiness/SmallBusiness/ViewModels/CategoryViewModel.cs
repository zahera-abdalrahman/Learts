using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.ViewModels
{
    public class CategoryViewModel
    {
        [Key]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category Name is required")]
        public string CategoryName { get; set; }

        [Required]
        public string Image { get; set; }

        public IFormFile File { get; set; }
    }
}
