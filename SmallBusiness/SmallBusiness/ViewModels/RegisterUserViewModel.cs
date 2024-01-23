using SmallBusiness.List;
using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        [Display(Name = "City")]
        public string City { get; set; }
        public string Image { get; set; } = "2024.png";

    }
}
