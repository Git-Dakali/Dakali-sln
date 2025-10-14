using Dakali.Interface;

namespace Dakali.Domine.Base
{
    public class Entity : IEntity
    {
        public long Id { get; set; }

        public string SearchString { get { return ToString(); } }
    }
}
