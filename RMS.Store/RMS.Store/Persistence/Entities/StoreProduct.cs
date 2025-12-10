using System;
using System.ComponentModel.DataAnnotations;

namespace RMS.Store.Persistence.Entities
{
    public class StoreProduct
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal MinPrice { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
