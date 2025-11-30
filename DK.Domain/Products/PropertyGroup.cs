using Dakali.Domine.Base;
using System.Collections.Generic;
using System.Linq;

namespace DK.Domain.Products
{
    public class PropertyGroup : EntityGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<Property> Properties { get; set; }

        public PropertyGroup() { 
            Name = string.Empty;
            SortOrder = 1;
            Properties = new List<Property>();
        }

        public override string ToString()
        {
            return $"{Name} [ {string.Join(" , ", Properties.Select(a => a.ToString()))} ]";
        }
    }
}
