namespace CheerfulChomps.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Image { get; set; }

        // FK: every Product belongs to 1 Category
        public int CategoryId { get; set; }

        // Parent ref to Category that the Product belongs to
        public Category Category { get; set; }
    }
}
