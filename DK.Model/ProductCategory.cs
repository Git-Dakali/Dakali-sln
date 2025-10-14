using Dakali.Domine.Base;

namespace DK.Model
{
    public class ProductCategory : EntityCode
    {
        public string Name { get; set; }

        public ProductCategory()
        { 
            Name = string.Empty;
        }

        public override string ToString()
        {
            return $"{Code} {Name}";
        }
    }
}
