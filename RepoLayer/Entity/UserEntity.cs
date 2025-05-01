using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RepoLayer.Entity
{
    public class UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } = "User";

        [NotMapped]
        public string Token { get; set; }

        // Navigation properties
        public virtual ICollection<BookEntity> Books { get; set; }
        public virtual ICollection<CartEntity> Carts { get; set; }
        public virtual ICollection<WishlistEntity> Wishlists { get; set; }
        public virtual ICollection<AddressEntity> Addresses { get; set; }
    }
}
