using Dakali.Domine.Base;
using System.Collections.Generic;

namespace DK.Domain.Products
{
    public class Product : EntityGuid
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Model Model { get; set; }
        public IEnumerable<Variant> Variants { get; set; }

        public Product() {
            Name = string.Empty;
            Description = string.Empty;
            Variants = new List<Variant>();
        }

        public override string ToString()
        {
            return $"{Model?.Code ?? string.Empty} {Name}";
        }
    }
}
