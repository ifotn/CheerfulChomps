using CheerfulChomps.Data;
using CheerfulChomps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            // fetch list of products for display, joining to parent Category & sorting a-z
            var products = _context.Product
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToList();

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
                // fetch Categories a-z into dropdown list for parent selection on the form
                ViewBag.CategoryId = new SelectList(_context.Category.OrderBy(c => c.Name).ToList(), "CategoryId", "Name");
                return View(product);
            }

            // save to db
            _context.Product.Add(product);
            _context.SaveChanges();

            // refresh product list
            return RedirectToAction("Index");
        }

        // GET: /Products/Edit/7 => display populated Product form
        public IActionResult Edit(int id)
        {
            // find Product
            var product = _context.Product.Find(id);

            // if invalid id
            if (product == null)
            {
                return NotFound();
            }

            // fetch Categories a-z into dropdown list for parent selection on the form
            ViewBag.CategoryId = new SelectList(_context.Category.OrderBy(c => c.Name).ToList(), "CategoryId", "Name");

            // show view and pass selected Product data
            return View(product);
        }

        // POST: /Products/Edit/7 => update selected Product
        [HttpPost]
        public IActionResult Edit(int id, [Bind("ProductId,Name,Price,Stock,Image,CategoryId")] Product product)
        {
            // validate
            if (!ModelState.IsValid)
            {
                // fetch Categories a-z into dropdown list for parent selection on the form
                ViewBag.CategoryId = new SelectList(_context.Category.OrderBy(c => c.Name).ToList(), "CategoryId", "Name");
                return View(product);
            }

            // save to db
            _context.Product.Update(product);
            _context.SaveChanges();

            // refresh product list
            return RedirectToAction("Index");
        }

        // GET: /Products/Delete/8 => delete selected Product
        public IActionResult Delete(int id)
        {
            // find Product
            var product = _context.Product.Find(id);

            // if invalid id
            if (product == null)
            {
                return NotFound();
            }

            // delete & redirect
            _context.Product.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
