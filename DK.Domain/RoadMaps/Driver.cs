using Dakali.Domine.Base;

namespace DK.Domain.RoadMaps
{
    public class Driver : EntityGuid
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Dni { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName} {Dni}";
        }
    }
}
