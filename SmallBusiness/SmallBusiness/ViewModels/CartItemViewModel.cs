using SmallBusiness.Models;

namespace SmallBusiness.ViewModels
{
    public class CartItemViewModel
    {
        public int CartItemsId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; }
    }
}
