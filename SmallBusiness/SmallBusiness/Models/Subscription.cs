using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.Models
{
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }
        public string SellerId { get; set; }

        public Seller Seller { get; set; }


        [Required]
        [DataType(DataType.Date)]
        public DateTime SubscriptionStartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime SubscriptionEndDate { get; set; }

        public bool status { get; set; }

        public decimal Price { get; set; }


        //[Required(ErrorMessage = "Name should contain only letters and spaces")]
        //public string NameInCredit { get; set; }

        //public string PaymentIntentId { get; set; } // For Stripe payment intent ID

    }
}
