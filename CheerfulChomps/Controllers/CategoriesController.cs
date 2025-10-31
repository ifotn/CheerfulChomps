using CheerfulChomps.Data;
using CheerfulChomps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheerfulChomps.Controllers
{
    //[Authorize] // authentication check: block anonymous users
    [Authorize(Roles = "Administrator")]
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
            var categories = _context.Category.OrderBy(c => c.Name).ToList();

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
            // try to find selected Category to populate form
            var category = _context.Category.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            // pass category to view for display
            return View(category);
        }

        // POST: /Categories/Edit/27 => update Category and redirect to list
        [HttpPost]
        public IActionResult Edit([Bind("CategoryId,Name")] Category category)
        {
            // validate
            if (!ModelState.IsValid)
            {
                return View();
            }

            // save to db
            _context.Category.Update(category);
            _context.SaveChanges();

            // redirect
            return RedirectToAction("Index");
        }

        // GET: /Categories/Delete/27 => delete selected Category and refresh list
        public IActionResult Delete(int id)
        {
            var category = _context.Category.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            // remove from db
            _context.Category.Remove(category);
            _context.SaveChanges();

            // redirect
            return RedirectToAction("Index");
        }
    }
}
