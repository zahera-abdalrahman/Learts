using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.ViewModels
{

    public class PaymentViewModel
    {
        [Required]
        [CreditCard]
        [Display(Name = "Credit Card Number")]
        public string CCNum { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expiration Date")]
        public DateTime ExpireDate { get; set; }

        [Required]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "Invalid CVV")]
        public string CVV { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Address { get; set; }
    }
}