using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }

        [Required]
        public string? UserId { get; set; }

        public User User { get; set; }
        public int? ProductId { get; set; }

        public Product Product { get; set; }

        public bool IsFav { get; set; }


        //public int? ProfileId { get; set; }

        //[ForeignKey(nameof(ProfileId))]
        //public Profile Profile { get; set; }
    }
}
