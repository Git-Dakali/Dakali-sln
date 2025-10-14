using Dakali.Domine.Base;
using System.Collections.Generic;

namespace DK.Model
{
    public class Product : EntityGuid
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ProductModel Model { get; set; }
        public IList<ProductVariant> Variant { get; set; }

        public Product() {
            Name = string.Empty;
            Description = string.Empty;
            Variant = new List<ProductVariant>();
        }

        public override string ToString()
        {
            return $"{Model?.Code ?? string.Empty} {Name}";
        }
    }
}
