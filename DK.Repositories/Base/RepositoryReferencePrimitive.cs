using Dakali.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Base
{
    public abstract class RepositoryReferencePrimitive<Parent, T>
         where Parent : IEntity
    {
        public abstract Task<IEnumerable<T>> Get(Parent parent, CancellationToken cancellation = default);
        public abstract Task Delete(Parent parent, CancellationToken cancellation = default);
        protected abstract Task Create(Parent parent, T value, CancellationToken cancellation = default);
        protected abstract Task<IEnumerable<T>> Create(Parent parent, IEnumerable<T> values, CancellationToken cancellation = default);

        public async Task<IEnumerable<T>> SyncCollection(Parent parent, IEnumerable<T> values, CancellationToken cancellation = default)
        {
            var valuesPersisted = await Get(parent, cancellation);

            if (values.Any(value => !valuesPersisted.Any(x => x.Equals(value))) || valuesPersisted.Any(x => !values.Any(value => value.Equals(x))))
            {
                await Delete(parent, cancellation);
                return (await Create(parent, values, cancellation)).ToList();
            }

            return valuesPersisted;
        }
    }
}
