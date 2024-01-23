using SmallBusiness.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SmallBusiness.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }

        //public DbSet<Buyer> Buyer { get; set; }

        public DbSet<Seller> Seller { get; set; }

        public DbSet<Cart> Cart { get; set; }

        public DbSet<CartItems> CartItems { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderItems> OrderItems { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<Testimonial> Testimonial { get; set; }

        public DbSet<Profile> Profile { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Favorite> Favorite { get; set; }

        public DbSet<Subscription> Subscription { get; set; }
        public DbSet<Payment> Payment { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
         .HasOne(o => o.Cart)
         .WithMany(c => c.Orders)
         .HasForeignKey(o => o.CartId)
         .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Seller>().ToTable("Sellers");

        }
    }
}
