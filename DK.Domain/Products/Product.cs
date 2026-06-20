using Dakali.Domine.Base;
using System.Collections.Generic;

namespace DK.Domain.Products
{
    public class Product : EntityCode
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Weight { get; set; }
        public Category Category { get; set; }
        public IEnumerable<Field> Fields { get; set; }
        public IEnumerable<Variant> Variants { get; set; }
        public IEnumerable<ProductColor> Colors { get; set; }
        public IEnumerable<ProductSku> Skus { get; set; }

        public Product() {
            Name = string.Empty;
            Description = string.Empty;
            Fields = new List<Field>();
            Variants = new List<Variant>();
            Colors = new List<ProductColor>();
            Skus = new List<ProductSku>();
        }

        public override string ToString()
        {
            return $"{Code} {Name} {Category?.ToString() ?? string.Empty}";
        }
    }
}
