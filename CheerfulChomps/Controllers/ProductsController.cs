using Microsoft.AspNetCore.Mvc;

namespace CheerfulChomps.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
