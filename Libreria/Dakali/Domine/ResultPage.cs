using Dakali.Interface;
using System.Collections.Generic;

namespace Dakali.Domine
{
    public class ResultPage<T> where T : IEntity
    {
        public IEnumerable<T> Values { get; set; }
        public long Count { get; set; }
    }
}
