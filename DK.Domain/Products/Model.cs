using Dakali.Domine.Base;
using System.Collections.Generic;

namespace DK.Domain.Products
{
    public class Model: EntityCode
    {
        public Category Category { get; set; }
        public IEnumerable<FieldGroup> FieldGroups { get; set; }
        public IEnumerable<Size> Sizes { get; set; }

        public Model() 
        {
            FieldGroups = new List<FieldGroup>();
            Sizes = new List<Size>();
        }

        public override string ToString()
        {
            return $"{Code} [{string.Join(", ", Sizes)}]";
        }
    }
}
