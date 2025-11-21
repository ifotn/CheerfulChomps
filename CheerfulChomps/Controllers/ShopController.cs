using CheerfulChomps.Data;
using CheerfulChomps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;

namespace CheerfulChomps.Controllers
{
    public class ShopController : Controller
    {
        // shared db conn
        private readonly ApplicationDbContext _context;

        // config object to read vars from appsettings
        private IConfiguration _configuration;

        // enable db & configuration dependencies for this controller
        public ShopController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

        // GET: /Shop/Checkout => show empty Checkout form to get Customer info
        // Customer must log in here to continue
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        // POST: /Shop/Checkout => save order info to Session var. We'll save to db after payment.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout([Bind("FirstName,LastName,Address,City,Province,PostalCode,Phone")] Order order)
        {
            // auto-fill date, total, email
            order.OrderDate = DateTime.Now;
            order.CustomerId = User.Identity.Name;
            order.OrderTotal = (from c in _context.CartItem
                                where c.CustomerId == GetCustomerId()
                                select c.Quantity * c.Price).Sum();

            // save order to Session var so we can save to db after payment
            HttpContext.Session.SetObject("Order", order);
            HttpContext.Session.SetInt32("OrderTotal", (int)(order.OrderTotal * 100));

            return RedirectToAction("Payment");
        }

        // GET: /Shop/Payment => invoke Stripe Payment page
        [Authorize]
        public IActionResult Payment()
        {
            // get stripe key from config.  This requires an IConfiguration dependency to read appsettings
            StripeConfiguration.ApiKey = _configuration["StripeSecretKey"];

            // set up payment options for Stripe checkout session
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "cad",
                            UnitAmount = HttpContext.Session.GetInt32("OrderTotal"),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Cheerful Chomps Purchase"
                            }
                        },
                        Quantity = 1
                    }                   
                },
                Mode = "payment",
                PaymentMethodTypes = new List<string> { "card" },
                SuccessUrl = "https://" + Request.Host + "/Shop/SaveOrder",
                CancelUrl = "https://" + Request.Host + "/Shop/Cart"
            };

            // NOW invoke Stripe
            var service = new SessionService();
            Session session = service.Create(options);

            // after done at stripe, redirect to either Success or Cancel Url (303)
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303); 
        }

        // GET: /Shop/SaveOrder => save order & details, empty cart, show confirmation
        [Authorize]
        public IActionResult SaveOrder()
        {
            // create order from session var
            var order = HttpContext.Session.GetObject<Order>("Order");
            _context.Order.Add(order);
            _context.SaveChanges();

            // move cart items to order details then empty cart
            var cartItems = _context.CartItem.Where(c => c.CustomerId == GetCustomerId());
            foreach (var item in cartItems)
            {
                // create new Order Detail
                var orderDetail = new OrderDetail
                {
                    Quantity = item.Quantity,
                    ProductId = item.ProductId,
                    Price = item.Price,
                    OrderId = order.OrderId
                };

                _context.OrderDetail.Add(orderDetail);
                _context.CartItem.Remove(item);
            }
            _context.SaveChanges();

            // clear session vars (all 4)
            HttpContext.Session.Clear();

            // redirect to order confirmation => /Orders/Details/123
            return RedirectToAction("Details", "Orders",  new { @id = order.OrderId });
        }
    }
}
