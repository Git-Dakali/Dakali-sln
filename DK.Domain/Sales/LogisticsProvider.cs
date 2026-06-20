using Dakali.Domine.Base;

namespace DK.Domain.Sales
{
    public class LogisticsProvider : EntityCode
    {
        public string Name { get; set; }
        public bool IsInHouse { get; set; }

        public LogisticsProvider()
            : base()
        {
            Name = string.Empty;
            IsInHouse = false;
        }

        public override string ToString()
        {
            return $"{Code} {Name}";
        }
    }
}
