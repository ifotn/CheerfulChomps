namespace CheerfulChomps.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        // child ref to list of products: 1 Category / Many Products
        // must be nullable ? so we can first create a Category without Products, then attach Products after
        public List<Product>? Products { get; set; }
    }
}
