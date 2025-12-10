using System.ComponentModel.DataAnnotations;

namespace RMS.CentralApp.Infrastructure.Persistence.Entities
{
    public class StoreProduct
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required]
        public string Store { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal MinPrice { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime UpdatedOn { get; set; }
    }
}
