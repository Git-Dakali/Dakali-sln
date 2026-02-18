using Dakali.Domine.Base;

namespace DK.Domain.GeographicLocation
{
    public class City : EntityGuid
    {
        public string Name { get; set; }
        public string ZipCode { get; set; }
        public Province Province { get; set; }

        public City() 
            :base()
        {
            Name = string.Empty;
        }
    }
}
