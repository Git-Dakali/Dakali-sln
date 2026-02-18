using Dakali.Domine.Base;

namespace DK.Domain.GeographicLocation
{
    public class Country : EntityCode
    {
        public string Name { get; set; }
        public Country()
            :base()
        {
            Name = string.Empty;
        }
    }
}
