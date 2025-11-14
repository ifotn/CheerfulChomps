using System.ComponentModel.DataAnnotations;

namespace CheerfulChomps.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Price { get; set; }

        // FK
        [Required]
        public int OrderId { get; set; }

        // FK
        [Required]
        public int ProductId { get; set; }

        // parent refs
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
