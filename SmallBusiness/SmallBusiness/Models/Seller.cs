using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.Models
{
    public class Seller:User
    {
              
        [Required]
        public bool IsApproved { get; set; }
        public Profile Profile { get; set; }

        [DataType(DataType.Date)]
        public DateTime RegisterDate { get; set; }


    }
}
