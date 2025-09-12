using CheerfulChomps.Models;
using Microsoft.AspNetCore.Mvc;

namespace CheerfulChomps.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            // create mock list of categories in memory
            var categories = new List<Category>();

            for (var i = 1; i < 16; i++)
            {
                categories.Add(new Category { CategoryId = i, Name = "Category " + i.ToString() });
            }

            // pass list to view for display
            return View(categories);
        }

        // GET: /Shop/ByCategory/7
        public IActionResult ByCategory(int id)
        {
            // we will look up the Category by id from the db in Week 4; mock the value for now
            ViewData["Category"] = "Category " + id.ToString();

            return View();
        }
    }
}
