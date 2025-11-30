using Dakali.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Base
{
    public abstract class RepositoryReferencePrimitive<Parent, T>
         where Parent : IEntity
    {
        public abstract Task<IEnumerable<T>> Get(Parent parent, CancellationToken cancellation = default);
        public abstract Task Delete(Parent parent, CancellationToken cancellation = default);
        public abstract Task<IEnumerable<T>> Create(Parent parent, IEnumerable<T> values, CancellationToken cancellation = default);

        public async Task<IEnumerable<T>> SyncCollection(Parent parent, IEnumerable<T> values, CancellationToken cancellation = default)
        {
            await Delete(parent, cancellation);
            await Create(parent, values, cancellation);

            return await Get(parent, cancellation);
        }
    }
}
