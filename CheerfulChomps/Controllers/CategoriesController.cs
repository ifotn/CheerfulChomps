using CheerfulChomps.Models;
using Microsoft.AspNetCore.Mvc;

namespace CheerfulChomps.Controllers
{
    public class CategoriesController : Controller
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

        public IActionResult Create()
        {
            return View();
        }

        // GET: /Categories/Edit/27 => look up Category based on id param so user can Edit it
        public IActionResult Edit(int id)
        {
            return View();
        }
    }
}
