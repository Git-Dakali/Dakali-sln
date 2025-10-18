using Dakali.Domine.Base;

namespace DK.Domain.Products
{
    public class Category : EntityCode
    {
        public string Name { get; set; }

        public Category()
        { 
            Name = string.Empty;
        }

        public override string ToString()
        {
            return $"{Code} {Name}";
        }
    }
}
