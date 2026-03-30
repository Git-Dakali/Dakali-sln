using Dakali.Domine.Base;
using System.Collections.Generic;

namespace DK.Domain.Products
{
    public class ProductColor : EntityGuid
    {
        public string Name { get; set; }
        public string Hex { get; set; }
        public string Sku { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<Image> Images { get; set; }

        public ProductColor()
        {
            Name = string.Empty;
            Hex = string.Empty;
            SortOrder = 1;
            Images = new List<Image>();
        }

        public override string ToString()
        {
            return $"{Name} {Sku}";
        }
    }
}
