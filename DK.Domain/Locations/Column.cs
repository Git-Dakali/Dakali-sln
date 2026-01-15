using Dakali.Domine.Base;

namespace DK.Domain.Locations
{
    public class Column : EntityCode
    {
        public string Name { get; set; }
        public Column() 
        {
            Name = string.Empty;
        }

        public override string ToString()
        {
            return $"{Code} {Name}";
        }
    }
}
