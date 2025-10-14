using Dakali.Interface;
using System.Diagnostics.CodeAnalysis;

namespace Dakali.Domine.Base
{
    public class EntityCode : EntityGuid, IEntityCode
    {
        public EntityCode()
            : base()
        {
            Code = string.Empty;
        }

        public string Code { get; set; }

        public override string ToString()
        {
            return Code;
        }
    }
}
