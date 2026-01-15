using Dakali.Domine.Base;

namespace DK.Domain.Locations
{
    public class Location : EntityGuid
    {
        public Hallway Hallway { get; set; }
        public Column Column { get; set; }
        public Level Level { get; set; }
        public LocationState State { get; set; }

        public Location() 
        {
        }

        public override string ToString()
        {
            return $"{Hallway.ToString()} {Column.ToString()} {Level.ToString()} {State.ToString()}";
        }
    }
}
