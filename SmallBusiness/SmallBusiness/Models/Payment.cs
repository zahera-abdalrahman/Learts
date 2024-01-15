using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string BuyerID { get; set; }
        public User Buyer { get; set; }
        public string Status { get; set; }

        [Required(ErrorMessage = "Transaction date is required")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Order ID is required")]
        public int OrderId { get; set; }

        public Order Order { get; set; }
    }
}
