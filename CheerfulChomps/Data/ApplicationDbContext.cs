using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CheerfulChomps.Models;

namespace CheerfulChomps.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CheerfulChomps.Models.Category> Category { get; set; } = default!;
        public DbSet<CheerfulChomps.Models.Product> Product { get; set; } = default!;
        public DbSet<CartItem> CartItem { get; set; }  
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
    }
}
