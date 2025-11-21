using Dakali.Domine.Base;
using System.Collections.Generic;

namespace DK.Domain.Products
{
    public class Variant : EntityGuid
    {
        public string Size { get; set; }
        public decimal Cost { get; set; }
        public IList<Color> ColorsHex { get; set; }
        public IList<Image> Images { get; set; }
        public IList<Attribute> Attributes { get; set; }

        public Variant()
        {
            ColorsHex = new List<Color>();
            Attributes = new List<Attribute>();
            Images = new List<Image>();
        }

        public override string ToString()
        {
            return $"{Size}";
        }
    }
}
