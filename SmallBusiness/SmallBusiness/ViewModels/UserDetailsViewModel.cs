using SmallBusiness.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SmallBusiness.ViewModels
{
    public class UserDetailsViewModel
    {
        public User User { get; set; }
        public List<Order> Orders { get; set; }

        [Display(Name = "Edit Details")]
        public UpdateViewModel EditModel { get; set; }

    }
}
