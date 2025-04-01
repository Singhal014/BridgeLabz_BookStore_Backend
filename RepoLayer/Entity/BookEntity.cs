using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepoLayer.Entity
{
    public class BookEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Image { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string AuthorName { get; set; }

        public int Quantity { get; set; }

        public int Price { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public UserEntity? User { get; set; }
    }
}
