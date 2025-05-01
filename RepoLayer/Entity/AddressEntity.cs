using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RepoLayer.Entity
{

    public class AddressEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public int PhoneNo { get; set; }

        public int PinCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Address { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserEntity User { get; set; }
    }
}