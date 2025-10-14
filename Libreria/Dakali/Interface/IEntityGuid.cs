using System;

namespace Dakali.Interface
{
    public interface IEntityGuid : IEntity
    {
        long Id { get; set; }
        DateTime CreationDate { get; set; }
        DateTime? RemoveDate { get; set; }
        DateTime UpdateDate { get; set; }
        long Version { get; set; }
        Guid Guid { get; set; }
        bool IsDeleted { get; set; }
    }
}
