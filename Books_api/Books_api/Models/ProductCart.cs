namespace Books_api.Models
{
    public class CartDataDetails
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int ProductId { get; set; }
        public int ProductCount { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public DateTime EntryDate { get; set; }
        public bool Status { get; set; }
        public string Remarks { get; set; }
    }

    public class AddToCartParams
    {
        public int ProductId { get; set; }
    }

    public class UpdateCartProductCount
    {
        public int CartId { get; set; }
        public int Count { get; set; }
    }

    public class UpdateAndDeleteCartProductStatus
    {
        public int CartId { get; set; }
    }
}
