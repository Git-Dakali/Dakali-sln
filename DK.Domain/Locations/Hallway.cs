using Dakali.Domine.Base;

namespace DK.Domain.Locations
{
    public class Hallway : EntityCode
    {
        public string Name { get; set; }

        public Hallway() 
        { 
            Name = string.Empty;
        }

        public override string ToString()
        {
            return $"{Code} {Name}";
        }
    }
}
