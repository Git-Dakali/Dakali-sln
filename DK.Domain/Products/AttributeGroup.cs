using Dakali.Domine.Base;
using System.Collections.Generic;
using System.Linq;

namespace DK.Domain.Products
{
    public class AttributeGroup : EntityGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<Attribute> Attributes { get; set; }

        public AttributeGroup() { 
            Name = string.Empty;
            SortOrder = 0;
            Attributes = new List<Attribute>();
        }

        public override string ToString()
        {
            return $"{Name} [ {string.Join(" , ", Attributes.Select(a => a.ToString()))} ]";
        }
    }
}
