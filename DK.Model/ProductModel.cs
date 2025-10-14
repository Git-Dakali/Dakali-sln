using Dakali.Domine.Base;
using System.Collections.Generic;

namespace DK.Model
{
    public class ProductModel: EntityCode
    {
        public ProductCategory Category { get; set; }
        public IList<ProductFieldGroup> FieldGroups { get; set; }
        public List<string> Size { get; set; }

        public ProductModel() 
        {
            FieldGroups = new List<ProductFieldGroup>();
            Size = new List<string>();
        }

        public override string ToString()
        {
            return $"{Code} [{string.Join(", ", Size)}]";
        }
    }
}
