using Dakali.Domine.Base;
using DK.Domain.Products;

namespace DK.Domain.Sales
{
    public class SaleDetail : EntityGuid
    {
        public Product Product { get; set; }
        public Variant Variant { get; set; }
        public ProductColor Color { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public bool IsExtra { get; set; }
        public Stock Stock { get; set; }
    }
}
