using Dakali.Interface;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Base
{
    public abstract class RepositoryReferenceEntity<Parent, Child> : IRepositoryReferenceEntity<Parent, Child>
         where Parent : IEntity
        where Child : IEntity
    {
        public abstract Task<Child> Create(Parent parent, Child entity, CancellationToken cancellation = default);
        public abstract Task Delete(Parent parent, Child entity, CancellationToken cancellation = default);
        public abstract Task Delete(Parent parent, IEnumerable<Child> entities, CancellationToken cancellation = default);
        public abstract Task<Child> Get(Parent parent, long id, CancellationToken cancellation = default);
        public abstract Task<IEnumerable<Child>> Get(Parent parent, CancellationToken cancellation = default);
        public abstract Task<Child> Update(Parent parent, Child entity, CancellationToken cancellation = default);
        public abstract Task<bool> HasChanges(Child entity, Child persited);

        public async Task<IEnumerable<Child>> SyncCollection(Parent parent, IEnumerable<Child> entities, CancellationToken cancellation = default)
        {

            var listPersisted = await Get(parent, cancellation);
            foreach (var persisted in listPersisted)
            {
                var entity = entities.SingleOrDefault(x => x.Id > 0 && x.Id == persisted.Id);

                if (entity is null)
                    await Delete(parent, persisted, cancellation);
                else if(await HasChanges(entity, persisted))
                    await Update(parent, entity, cancellation);
            }

            foreach (var entity in entities.Where(x => x.Id == 0))
            {
                await Create(parent, entity);
            }

            return await Get(parent);
        }

        public async Task<bool> HasChanges(Parent parent, IEnumerable<Child> entities, CancellationToken cancellation = default)
        {
            var listPersisted = await Get(parent, cancellation);
            foreach (var persisted in listPersisted)
            {
                var entity = entities.SingleOrDefault(x => x.Id > 0 && x.Id == persisted.Id);

                if (entity is null)
                    return true;
                else if (await HasChanges(entity, persisted))
                    return true;
            }

            return entities.Any(x => x.Id == 0);
        }
    }
}
