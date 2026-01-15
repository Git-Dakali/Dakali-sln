using Dakali.Domine.Base;

namespace DK.Domain
{
    public class City : EntityCode
    {
        public string Name { get; set; }

        public City() 
            :base()
        {
            Name = string.Empty;
        }
    }
}
