using Dakali.Domine.Base;

namespace DK.Domain
{
    public class Province : EntityCode
    {
        public string Name { get; set; }

        public Province()
            :base()
        {
            Name = string.Empty;
        }
    }
}
