using Dakali.Domine.Base;

namespace DK.Domain.Products
{
    public class Property : EntityGuid
    {
        public string Field { get; set; }
        public string Value { get; set; }

        public Property()
        {
            Field = string.Empty;
            Value = string.Empty;
        }

        public override string ToString()
        {
            return $"{Field} {Value}";
        }
    }
}
