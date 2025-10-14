using Dakali.Domine.Base;
using System.Collections.Generic;

namespace DK.Model
{
    public class ProductVariant : Entity
    {
        public string Size { get; set; }
        public decimal Cost { get; set; }
        public IList<string> ColorHex { get; set; }
        public IList<ProductImage> Images { get; set; }
        public IList<ProductAttribute> Attributes { get; set; }

        public ProductVariant()
        {
            Size = string.Empty;
            ColorHex = new List<string>();
            Attributes = new List<ProductAttribute>();
            Images = new List<ProductImage>();
        }

        public override string ToString()
        {
            return $"{Size}";
        }
    }
}
