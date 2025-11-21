using Dakali.Domine.Base;

namespace DK.Domain.Products
{
    public class Color : EntityGuid
    {
        public string Name { get; set; }
        public string Hex { get; set; }
        public int SortOrder { get; set; }

        public Color()
        {
            Name = string.Empty;
            Hex = string.Empty;
            SortOrder = 1;
        }

        public override string ToString()
        {
            return $"{SortOrder}-{Hex}";
        }
    }
}
