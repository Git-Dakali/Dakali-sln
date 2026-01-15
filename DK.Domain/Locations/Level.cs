using Dakali.Domine.Base;

namespace DK.Domain.Locations
{
    public class Level : EntityCode
    {
        public string Name { get; set; }

        public Level() 
        {
            Name = string.Empty;
        }

        public override string ToString()
        {
            return $"{Code} {Name}";
        }
    }
}
