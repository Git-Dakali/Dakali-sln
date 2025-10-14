using Dakali.Domine.Base;

namespace DK.Model
{
    public class ProductAttribute : Entity
    {
        public string Field { get; set; }
        public string Value { get; set; }

        public ProductAttribute()
        {
            Field = string.Empty;
            Value = string.Empty;
        }
    }
}
