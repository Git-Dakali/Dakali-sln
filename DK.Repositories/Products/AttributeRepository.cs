using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class AttributeRepository : RepositoryReferenceEntity<AttributeGroup, Attribute>
    {
        private readonly ISession _session;

        public AttributeRepository(ISession session)
        {
            _session = session;
        }

        public override async Task<Attribute> Create(AttributeGroup parent, Attribute entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                INSERT INTO dbo.Attribute (AttributeGroupId, [Field], [Value], SearchString)
                OUTPUT INSERTED.Id, INSERTED.[Field], INSERTED.[Value],
                    INSERTED.SearchString, INSERTED.CreationDate, INSERTED.UpdateDate, INSERTED.RemoveDate,
                    INSERTED.Version, INSERTED.Guid, INSERTED.IsDeleted
                VALUES (@AttributeGroupId, @Field, @Value, @SearchString);";

            entity.SearchString = entity.ToString();
            return await _session.Connection.QuerySingleAsync<Attribute>(
                new CommandDefinition(sql, new { AttributeGroupId = parent.Id, entity.Field, entity.Value, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));
        }

        public override async Task Delete(AttributeGroup parent, Attribute entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.Attribute
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @Id AND AttributeGroupId = @AttributeGroupId AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(
                new CommandDefinition(sql, new { entity.Id, AttributeGroupId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public override async Task Delete(AttributeGroup parent, IEnumerable<Attribute> entities, CancellationToken cancellation = default)
        {
            if (entities is null)
                return;

            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }   

        public override async Task<Attribute?> Get(AttributeGroup parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT Id, [Field], [Value],
                    SearchString, CreationDate, UpdateDate, RemoveDate, Version, Guid, IsDeleted
                FROM dbo.Attribute
                WHERE  Id = @Id AND AttributeGroupId = @AttributeGroupId AND IsDeleted = 0";

            return await _session.Connection.QuerySingleOrDefaultAsync<Attribute>(
                new CommandDefinition(sql, new { Id = id, AttributeGroupId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public override async Task<IEnumerable<Attribute>> Get(AttributeGroup parent, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT Id, [Field], [Value],
                    SearchString, CreationDate, UpdateDate, RemoveDate, Version, Guid, IsDeleted
                FROM dbo.Attribute
                WHERE AttributeGroupId = @AttributeGroupId AND IsDeleted = 0
                ORDER BY Id;";

            return await _session.Connection.QueryAsync<Attribute>(
                new CommandDefinition(sql, new { AttributeGroupId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public override async Task<Attribute> Update(AttributeGroup parent, Attribute entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.Attribute
                    SET [Field] = @Field,
                       [Value] = @Value,
                       SearchString = @SearchString,
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                OUTPUT INSERTED.Id, INSERTED.[Field], INSERTED.[Value],
                       INSERTED.CreationDate, INSERTED.UpdateDate, INSERTED.RemoveDate,
                       INSERTED.Version, INSERTED.Guid, INSERTED.IsDeleted
                 WHERE Id = @Id AND AttributeGroupId = @AttributeGroupId AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var updated = await _session.Connection.QuerySingleOrDefaultAsync<Attribute>(
                new CommandDefinition(sql,
                    new { entity.Id, AttributeGroupId = parent.Id, entity.Field, entity.Value, entity.SearchString },
                    _session.Transaction,
                    cancellationToken: cancellation));

            return updated ?? throw new KeyNotFoundException($"El attributo {entity.Id}-{entity.Field} no encontrado para el grupo {parent.Id}-{parent.Name}.");
        }

        public async override Task<bool> HasChanges(Attribute entity, Attribute persited)
        {
            return entity.Id != persited.Id
                || string.Compare(entity.Field, persited.Field, true) != 0
                || string.Compare(entity.Value, persited.Value, true) != 0;
        }
    }
}
