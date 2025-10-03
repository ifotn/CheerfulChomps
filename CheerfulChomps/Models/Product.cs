using System.ComponentModel.DataAnnotations;

namespace CheerfulChomps.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:c}")] // 0 => the value to format, c => currency
        [Range(0,200)]
        public decimal Price { get; set; }

        public int Stock { get; set; }
        public string? Image { get; set; }

        // FK: every Product belongs to 1 Category
        public int CategoryId { get; set; }

        // Parent ref to Category that the Product belongs to
        public Category? Category { get; set; }
    }
}
