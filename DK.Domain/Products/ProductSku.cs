using Dakali.Domine.Base;

namespace DK.Domain.Products
{
    public class ProductSku : EntityGuid
    {
        public Product Product { get; set; }
        public ProductColor Color { get; set; }
        public Variant Variant { get; set; }
        public string Sku { get; set; }
    }
}
