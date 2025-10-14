using Dakali.Interface;
using System;

namespace Dakali.Domine.Base
{
    public class EntityGuid : Entity, IEntityGuid
    {
        public EntityGuid() { 
            Guid = Guid.NewGuid();
            CreationDate = DateTime.Now;
            UpdateDate = DateTime.Now;
            Version = 1;
            IsDeleted = false;
        }

        
        public DateTime CreationDate { get; set; }
        public DateTime? RemoveDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public long Version { get; set; }
        public Guid Guid { get; set; }
        public bool IsDeleted { get; set; }
    }
}
