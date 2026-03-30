using Dakali.Domine.Base;

namespace DK.Domain.Sales
{
    public class OriginSale : EntityCode
    {
        public string Name { get; set; }

        public OriginSale()
            :base()
        {
            Name = string.Empty;
        }

        public override string ToString()
        {
            return $"{Code} {Name}";
        }
    }
}
