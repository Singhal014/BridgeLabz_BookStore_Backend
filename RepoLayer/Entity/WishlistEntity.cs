using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RepoLayer.Entity
{
    public class WishlistEntity
    {
        [Key]
        public int WishlistId { get; set; }

        public int UserId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual UserEntity User { get; set; }

        [ForeignKey("BookId")]
        public virtual BookEntity Book { get; set; }
    }
}
