using Dakali.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Interface.Base
{
    public interface IRepository<T> where T : IEntity
    {
        Task<IEnumerable<T>> GetAll(CancellationToken cancellation = default);
        Task<T> Get(long id, CancellationToken cancellation = default);
        Task<T> Create(T entity, CancellationToken cancellation = default);
        Task<T> Update(T entity, CancellationToken cancellation = default);
        Task Delete(T entity, CancellationToken cancellation = default);
    }
}
