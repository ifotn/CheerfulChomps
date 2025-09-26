using CheerfulChomps.Data;
using CheerfulChomps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CheerfulChomps.Controllers
{
    public class ProductsController : Controller
    {
        // shared db connection
        ApplicationDbContext _context;

        // constructor w/db conn dependency
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // fetch list of products for display
            var products = _context.Product.ToList();

            return View(products);
        }

        // GET: /Products/Create => show empty Product form
        public IActionResult Create()
        {
            // fetch Categories a-z into dropdown list for parent selection on the form
            ViewBag.CategoryId = new SelectList(_context.Category.OrderBy(c => c.Name).ToList(), "CategoryId", "Name");

            return View();
        }

        // POST: /Products/Create => process form submission to create new Product
        [HttpPost]
        public IActionResult Create([Bind("Name,Price,Stock,Image,CategoryId")] Product product)
        {
            // validate
            if (!ModelState.IsValid)
            {
                return View();
            }

            // save to db
            _context.Product.Add(product);
            _context.SaveChanges();

            // refresh product list
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            return View();
        }
    }
}
