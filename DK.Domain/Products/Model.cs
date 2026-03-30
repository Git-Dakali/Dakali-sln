using Dakali.Domine.Base;
using System.Collections.Generic;

namespace DK.Domain.Products
{
    public class Model: EntityCode
    {
        public Category Category { get; set; }
        public IEnumerable<FieldGroup> FieldGroups { get; set; }
        public IEnumerable<string> VariantNames { get; set; }

        public Model() 
        {
            FieldGroups = new List<FieldGroup>();
            VariantNames = new List<string>();
        }

        public override string ToString()
        {
            return $"{Code} [ {string.Join(" , ", VariantNames)} ]";
        }
    }
}
