namespace ModelLayer.Models
{
    public class WishlistResponseModel
    {
        public int WishlistId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
    }
}