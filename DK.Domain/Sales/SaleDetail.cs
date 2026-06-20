using Dakali.Domine.Base;
using DK.Domain.Products;

namespace DK.Domain.Sales
{
    public class SaleDetail : EntityGuid
    {
        public Product Product { get; set; }
        public ProductSku ProductSku { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public bool IsExchangeItem { get; set; }
        public Stock Stock { get; set; }

        public override string ToString()
        {
            return $"{Product?.Code} {Product?.Name} {ProductSku?.Variant?.Name} {ProductSku?.Color?.Name}";
        }
    }
}
