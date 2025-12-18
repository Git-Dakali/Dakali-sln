using Dakali.Domine.Base;

namespace DK.Domain.Products
{
    public class StockState : EntityCode
    {
        public string Name { get; set; }

        public StockState()
        {
            Name = string.Empty;
        }

        public override string ToString()
        {
            return $"{Code} {Name}";
        }
    }
}
