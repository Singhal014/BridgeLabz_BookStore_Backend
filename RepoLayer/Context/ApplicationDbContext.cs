using Microsoft.EntityFrameworkCore;
using RepoLayer.Entity;

namespace RepoLayer.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserEntity>? Users { get; set; }
        public DbSet<BookEntity>? Books { get; set; }
        public DbSet<CartEntity>? Carts { get; set; }
        public DbSet<WishlistEntity>? Wishlists { get; set; }
        public DbSet<AddressEntity> ? Addresses { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }

    }
}
