using Dakali.Domine.Base;

namespace DK.Domain.Locations
{
    public class LocationState : EntityCode
    {
        public string Name { get; set; }

        public LocationState()
        {
            Name = string.Empty;
        }

        public override string ToString()
        {
            return $"{Code} {Name}";
        }
    }
}
