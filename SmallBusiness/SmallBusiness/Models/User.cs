using Microsoft.AspNetCore.Identity;
using SmallBusiness.List;
using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.Models
{
    public class User:IdentityUser
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
        public string Image { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
       
        //[Required]
        //public string Street { get; set; }

    }
}
