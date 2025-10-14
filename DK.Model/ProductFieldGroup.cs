using Dakali.Domine.Base;
using System.Collections.Generic;

namespace DK.Model
{
    public class ProductFieldGroup: EntityGuid
    {
        public string Name { get; set; }
        public IList<string> Fields { get; set; }

        public ProductFieldGroup() { 
            Name = string.Empty;
            Fields = new List<string>();
        }

        public override string ToString()
        {
            return $"{Name} [{string.Join(", ", Fields)}]";
        }
    }
}
