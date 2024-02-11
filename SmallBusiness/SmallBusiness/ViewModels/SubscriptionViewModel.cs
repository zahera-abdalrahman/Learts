using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.ViewModels
{
    public class SubscriptionViewModel
    {
        public int SubscriptionId { get; set; }


        [Required]
            public string SellerId { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime SubscriptionStartDate { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime SubscriptionEndDate { get; set; }

            [Required]
            public decimal Price { get; set; }

            public bool status { get; set; }

        [Required]
            [CreditCard(ErrorMessage = "Invalid credit card number")]
            public string CreditCardNumber { get; set; }

            [Required]
            [RegularExpression(@"^\d{2}\/\d{2}$", ErrorMessage = "Invalid expiry date")]
            public string CreditCardExpiry { get; set; }

            [Required]
            [RegularExpression(@"^\d{3}$", ErrorMessage = "Invalid CVC")]
            public string CreditCardCvc { get; set; }

        [Required(ErrorMessage = "Name should contain only letters and spaces")]

        public string NameInCredit { get; set; }
    }
    }
