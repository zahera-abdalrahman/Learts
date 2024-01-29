using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallBusiness.Models
{
    public class Testimonial
    {
        public int TestimonialId { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Please enter your email")]
        public string Email { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }

        //[Required(ErrorMessage = "Testimonial status is required")]
        //public int TestimonialStatus { get; set; }

        [Required(ErrorMessage = "Testimonial message is required")]
        public string TestimonialMessage { get; set; }

        public bool isActive { get; set; }
    }
}
