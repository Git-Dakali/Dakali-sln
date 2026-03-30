using Dakali.Domine.Base;

namespace DK.Domain.GeographicLocation
{
    public class Province : EntityCode
    {
        public string Name { get; set; }
        public Country Country { get; set; }

        public Province()
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
