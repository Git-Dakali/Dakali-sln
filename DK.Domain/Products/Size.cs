using Dakali.Domine.Base;

namespace DK.Domain.Products
{
    public class Size : EntityGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }

        public Size()
        {
            Name = string.Empty;
            SortOrder = 1;
        }

        public override string ToString()
        {
            return $"{SortOrder}-{Name}";
        }
    }
}
