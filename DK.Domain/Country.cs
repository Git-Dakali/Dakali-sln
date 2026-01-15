using Dakali.Domine.Base;

namespace DK.Domain
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
