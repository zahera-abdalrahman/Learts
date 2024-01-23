using SmallBusiness.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallBusiness.ViewModels
{
    public class ProfileViewModel
    {
        public string UserId { get; set; }



        [Key]
        public int ProfileId { get; set; }

        [Required]
        public string ProfileImage { get; set; }


        [Required]
        public string ShopName { get; set; }

        [Required]
        public string Description { get; set; }


        [Required]
        public bool IsApproved { get; set; }


        [ForeignKey("SellerId")]
        public string SellerId { get; set; }
        public Seller Seller { get; set; }

        public IFormFile File { get; set; }
    }
}
