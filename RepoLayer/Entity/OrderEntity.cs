using RepoLayer.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class OrderEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderId { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }

    [Required]
    [ForeignKey("Book")]
    public int BookId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string Status { get; set; } = "Placed";

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string PaymentMethod { get; set; } = "COD";

    // Navigation properties
    public virtual UserEntity User { get; set; }
    public virtual BookEntity Book { get; set; }
}
