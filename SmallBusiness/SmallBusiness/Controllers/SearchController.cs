using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmallBusiness.Data;
using SmallBusiness.ViewModels;

namespace SmallBusiness.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return PartialView("_SearchPartial");
        }
    }
}
