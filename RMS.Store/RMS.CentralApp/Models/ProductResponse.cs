namespace RMS.CentralApp.Models
{
    public class ProductResponse
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public decimal MinPrice { get; set; }
    }
}
