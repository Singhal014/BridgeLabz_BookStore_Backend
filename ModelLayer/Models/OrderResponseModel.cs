using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public class OrderResponseModel
    {
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string Image { get; set; }
    }
}
