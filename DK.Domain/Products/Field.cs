using Dakali.Domine.Base;

namespace DK.Domain.Products
{
    public class Field : EntityGuid
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }

        public Field()
        {
            Name = string.Empty;
            Value = string.Empty;
            SortOrder = 1;
        }

        public override string ToString()
        {
            return $"{Name} {Value}";
        }
    }
}
