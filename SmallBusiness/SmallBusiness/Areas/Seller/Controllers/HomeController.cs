using Microsoft.AspNetCore.Mvc;

namespace SmallBusiness.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
