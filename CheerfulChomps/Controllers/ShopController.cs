using CheerfulChomps.Data;
using CheerfulChomps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;

namespace CheerfulChomps.Controllers
{
    public class ShopController : Controller
    {
        // shared db conn
        private readonly ApplicationDbContext _context;

        // enable db dependency for this controller
        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //// create mock list of categories in memory
            //var categories = new List<Category>();

            //for (var i = 1; i < 16; i++)
            //{
            //    categories.Add(new Category { CategoryId = i, Name = "Category " + i.ToString() });
            //}

            // fetch Categories from db
            var categories = _context.Category.OrderBy(c => c.Name).ToList();

            // pass list to view for display
            return View(categories);
        }

        // GET: /Shop/ByCategory/7
        public IActionResult ByCategory(int id)
        {
            // fetch products in selected category
            var products = _context.Product.Where(p => p.CategoryId == id)
                .OrderBy(p => p.Name)
                .ToList();

            // we will look up the Category by id from the db in Week 4; mock the value for now
            //ViewData["Category"] = "Category " + id.ToString();
            var category = _context.Category.Find(id);

            // ensure CategoryId is valid
            if (category == null)
            {
                return NotFound();
            }
            ViewData["Category"] = category.Name;

            return View(products);
        }

        // POST: /Shop/AddToCart => save item to user's cart in db
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int Quantity, int ProductId)
        {
            // identify customer
            var customerId = GetCustomerId();

            // look up product price
            decimal price = 0;
            var product = _context.Product.Find(ProductId);
            price = product.Price;

            // is this product already in this user's cart?
            var cartItem = _context.CartItem
                .Where(c => c.CustomerId == customerId && c.ProductId == ProductId)
                .SingleOrDefault(); // only fetch 1 record

            // customer doesn't already have this product in their cart => ADD
            if (cartItem == null)
            {
                // add item to user's cart
                var newCartItem = new CartItem
                {
                    Quantity = Quantity,
                    ProductId = ProductId,
                    CustomerId = customerId,
                    Price = price
                };

                _context.CartItem.Add(newCartItem);
            }
            else
            {
                cartItem.Quantity += Quantity;  // update quantity 
                _context.CartItem.Update(cartItem);
            }

            _context.SaveChanges();

            // redirect to cart page
            return RedirectToAction("Cart");
        }

        private string GetCustomerId()
        {
            // check for CustomerId session var
            if (HttpContext.Session.GetString("CustomerId") == null)
            {
                // if no CustomerId session var, create one using a GUID
                HttpContext.Session.SetString("CustomerId", Guid.NewGuid().ToString());
            }

            // return CustomerId session var
            return HttpContext.Session.GetString("CustomerId");
        }

        // GET: /Shop/Cart => show current user's cart
        public IActionResult Cart()
        {
            // fetch items from user's cart in db
            var cartItems = _context.CartItem
                .Include(c => c.Product) // join to parent to include Product Name in page view
                .Where(c => c.CustomerId == GetCustomerId())
                .ToList();

            // calc item total for navbar badge using Session var
            var itemCount = (from c in cartItems
                             select c.Quantity).Sum();
            HttpContext.Session.SetInt32("ItemCount", itemCount);

            return View(cartItems);
        }

        // GET: /Shop/RemoveFromCart/34 => this is obvious no?
        public IActionResult RemoveFromCart(int id)
        {
            // delete from cart
            var cartItem = _context.CartItem.Find(id);
            _context.CartItem.Remove(cartItem);
            _context.SaveChanges();

            // refresh cart
            return RedirectToAction("Cart");
        }
    }
}
