using Dakali.Domine.Base;
using System.Collections.Generic;
using System.Linq;

namespace DK.Domain.Products
{
    public class Variant : EntityGuid
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public bool Active { get; set; }
        public IEnumerable<Color> ColorsHex { get; set; }
        public IEnumerable<AttributeGroup> AttributeGroups { get; set; }

        public Variant()
        {
            ColorsHex = new List<Color>();
            AttributeGroups = new List<AttributeGroup>();
        }

        public override string ToString()
        {
            return $"{Name} [ {string.Join(" , ", AttributeGroups.Select(a => a.ToString()))} ]";
        }
    }
}
