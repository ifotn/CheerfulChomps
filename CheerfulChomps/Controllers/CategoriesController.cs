using CheerfulChomps.Data;
using CheerfulChomps.Models;
using Microsoft.AspNetCore.Mvc;

namespace CheerfulChomps.Controllers
{
    public class CategoriesController : Controller
    {
        // shared db connection obj
        ApplicationDbContext _context;

        // constructor: every time controller instance created, pass in a db connection obj
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // create mock list of categories in memory
            //var categories = new List<Category>();

            //for (var i = 1; i < 16; i++)
            //{
            //    categories.Add(new Category { CategoryId = i, Name = "Category " + i.ToString() });
            //}

            // fetch categories from db using DbSet
            var categories = _context.Category.ToList();

            // pass list to view for display
            return View(categories);
        }

        // GET: /Categories/Create => show empty Category form
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Categories/Create => process form submission 
        [HttpPost]
        public IActionResult Create([Bind("Name")] Category category)
        {
            // validate input
            if (!ModelState.IsValid)
            {
                // show user the form again to fix their mistakes
                return View();
            }

            // save to db using DbSet object
            _context.Category.Add(category);
            _context.SaveChanges();

            // redirect to updated list on Index
            return RedirectToAction("Index");
        }

        // GET: /Categories/Edit/27 => look up Category based on id param so user can Edit it
        public IActionResult Edit(int id)
        {
            return View();
        }
    }
}
