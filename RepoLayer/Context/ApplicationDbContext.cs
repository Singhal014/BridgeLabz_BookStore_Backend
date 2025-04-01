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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Book and User Relationship
            modelBuilder.Entity<BookEntity>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cart and User Relationship
            modelBuilder.Entity<CartEntity>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Cart and Book Relationship
            modelBuilder.Entity<CartEntity>()
                .HasOne(c => c.Book)
                .WithMany()
                .HasForeignKey(c => c.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Wishlist and User Relationship
            modelBuilder.Entity<WishlistEntity>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Wishlist and Book Relationship
            modelBuilder.Entity<WishlistEntity>()
                .HasOne(w => w.Book)
                .WithMany()
                .HasForeignKey(w => w.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
