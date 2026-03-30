using Dakali.Domine.Base;

namespace DK.Domain.Sales
{
    public class TaxStatus : EntityCode
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Code} {Name}";
        }
    }
}
