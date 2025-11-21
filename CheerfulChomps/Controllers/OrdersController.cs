using CheerfulChomps.Data;
using CheerfulChomps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CheerfulChomps.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        // shared db conn
        private readonly ApplicationDbContext _context;

        // constructor w/db dependency
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = new List<Order>();

            // fetch orders from db
            if (User.IsInRole("Administrator"))
            {
                orders = _context.Order.OrderByDescending(o => o.OrderId)
                    .ToList();
            }
            else
            {
                // filter for logged in user only
                orders = _context.Order.Where(o => o.CustomerId == User.Identity.Name)
                    .OrderByDescending(o => o.OrderId)
                    .ToList();
            }

            return View(orders);
        }

        // GET: /Orders/Details => show full Order + All Details
        public IActionResult Details(int id)
        {
            // fetch order from db
            var order = _context.Order
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == id);
            
            if (!User.IsInRole("Administrator"))
            {
                // verify logged in customer owns this order
                if (order.CustomerId != User.Identity.Name)
                {
                    return Unauthorized();
                }
            }

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
