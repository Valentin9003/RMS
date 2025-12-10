using System.ComponentModel.DataAnnotations;

namespace RMS.CentralApp.Models
{
    public class ProductRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = default!;

        [Required]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "MinPrice must be greater than zero.")]
        public decimal MinPrice { get; set; }
    }
}
