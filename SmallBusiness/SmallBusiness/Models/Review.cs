using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallBusiness.Models
{
    public class Review 
    {
        public int ReviewId { get; set; }

        [Required(ErrorMessage = "Review message is required")]
        public string ReviewMessage { get; set; }

        [Range(0, 5, ErrorMessage = "Review rate must be between 0 and 5")]
        public int ReviewRate { get; set; }

        [Required(ErrorMessage = "Review date is required")]
        public DateTime ReviewDate { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        //[Required(ErrorMessage = "Testimonial status is required")]
        //public int ReviewStatus { get; set; }

        [Required]
        public string? UserId { get; set; }

        public User User { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public bool isActive { get; set; }



    }
}
