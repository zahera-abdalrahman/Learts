namespace SmallBusiness.ViewModels
{
    public class ShoppingCartViewModel
    {
        public int CartId { get; set; }
        public List<CartItemViewModel> CartItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
