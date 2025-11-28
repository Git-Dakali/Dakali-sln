using Dakali.Domine.Base;

namespace DK.Domain.Products
{
    public class Attribute : EntityGuid
    {
        public string Field { get; set; }
        public string Value { get; set; }

        public Attribute()
        {
            Field = string.Empty;
            Value = string.Empty;
        }
    }
}
