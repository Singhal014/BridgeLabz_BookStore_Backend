using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepoLayer.Entity
{
    public class CartEntity
    {
        [Key]
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate { get; set; }
        public bool IsOrdered { get; set; }
        public bool IsRemoved { get; set; }

        [ForeignKey("UserId")]
        public virtual UserEntity User { get; set; }

        [ForeignKey("BookId")]
        public virtual BookEntity Book { get; set; }
    }
}
