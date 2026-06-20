using Dakali.Domine.Base;

namespace DK.Domain.Products
{
    public class Variant : EntityGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }

        public Variant()
        {
            Name = string.Empty;
            SortOrder = 1;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
