using SmallBusiness.Models;

namespace SmallBusiness.ViewModels
{
    public class cartViewModel
    {
        public int CartItemsCount { get; set; }
        public List<CartItems> CartItems { get; set; }
    }
}
