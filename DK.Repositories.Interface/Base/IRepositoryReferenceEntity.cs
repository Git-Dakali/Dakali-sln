using Dakali.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Interface.Base
{
    public interface IRepositoryReferenceEntity<Parent, Child> 
        where Parent : IEntity 
        where Child : IEntity
    {
        Task<Child> Get(Parent parent, long id, CancellationToken cancellation = default);
        Task<IEnumerable<Child>> Get(Parent parent, CancellationToken cancellation = default);
        Task<Child> Create(Parent parent, Child entity, CancellationToken cancellation = default);
        Task<Child> Update(Parent parent, Child entity, CancellationToken cancellation = default);
        Task Delete(Parent parent, Child entity, CancellationToken cancellation = default);
        Task Delete(Parent parent, IEnumerable<Child> entities, CancellationToken cancellation = default);
        Task<IEnumerable<Child>> SyncCollection(Parent parent, IEnumerable<Child> entities, CancellationToken cancellation = default);
    }
}
