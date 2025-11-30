using Dakali.Domine.Base;
using System.Collections.Generic;

namespace DK.Domain.Products
{
    public class FieldGroup : EntityGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<Field> Fields { get; set; }

        public FieldGroup()
        {
            Name = string.Empty;
            Fields = new List<Field>();
            SortOrder = 1;
        }

        public override string ToString()
        {
            return $"{Name} [{string.Join(", ", Fields)}]";
        }
    }
}
