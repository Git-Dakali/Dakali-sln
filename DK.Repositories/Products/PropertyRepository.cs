using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class PropertyRepository : RepositoryReferenceEntity<PropertyGroup, Property>
    {
        private readonly ISession _session;

        public PropertyRepository(ISession session)
        {
            _session = session;
        }

        public override async Task<Property> Create(PropertyGroup parent, Property entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                INSERT INTO dbo.Property (PropertyGroupId, [Field], [Value], SearchString)
                OUTPUT INSERTED.Id, INSERTED.[Field], INSERTED.[Value],
                    INSERTED.SearchString, INSERTED.CreationDate, INSERTED.UpdateDate, INSERTED.RemoveDate,
                    INSERTED.Version, INSERTED.Guid, INSERTED.IsDeleted
                VALUES (@PropertyGroupId, @Field, @Value, @SearchString);";

            entity.SearchString = entity.ToString();
            return await _session.Connection.QuerySingleAsync<Property>(
                new CommandDefinition(sql, new { PropertyGroupId = parent.Id, entity.Field, entity.Value, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));
        }

        public override async Task Delete(PropertyGroup parent, Property entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.Property
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @Id AND PropertyGroupId = @PropertyGroupId AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(
                new CommandDefinition(sql, new { entity.Id, PropertyGroupId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public override async Task Delete(PropertyGroup parent, IEnumerable<Property> entities, CancellationToken cancellation = default)
        {
            if (entities is null)
                return;

            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }   

        public override async Task<Property?> Get(PropertyGroup parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT Id, [Field], [Value],
                    SearchString, CreationDate, UpdateDate, RemoveDate, Version, Guid, IsDeleted
                FROM dbo.Property
                WHERE  Id = @Id AND PropertyGroupId = @PropertyGroupId AND IsDeleted = 0";

            return await _session.Connection.QuerySingleOrDefaultAsync<Property>(
                new CommandDefinition(sql, new { Id = id, PropertyGroupId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public override async Task<IEnumerable<Property>> Get(PropertyGroup parent, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT Id, [Field], [Value],
                    SearchString, CreationDate, UpdateDate, RemoveDate, Version, Guid, IsDeleted
                FROM dbo.Property
                WHERE PropertyGroupId = @PropertyGroupId AND IsDeleted = 0
                ORDER BY Id;";

            return await _session.Connection.QueryAsync<Property>(
                new CommandDefinition(sql, new { PropertyGroupId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public override async Task<Property> Update(PropertyGroup parent, Property entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.Property
                    SET [Field] = @Field,
                       [Value] = @Value,
                       SearchString = @SearchString,
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                OUTPUT INSERTED.Id, INSERTED.[Field], INSERTED.[Value],
                       INSERTED.CreationDate, INSERTED.UpdateDate, INSERTED.RemoveDate,
                       INSERTED.Version, INSERTED.Guid, INSERTED.IsDeleted
                 WHERE Id = @Id AND PropertyGroupId = @PropertyGroupId AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var updated = await _session.Connection.QuerySingleOrDefaultAsync<Property>(
                new CommandDefinition(sql,
                    new { entity.Id, PropertyGroupId = parent.Id, entity.Field, entity.Value, entity.SearchString },
                    _session.Transaction,
                    cancellationToken: cancellation));

            return updated ?? throw new KeyNotFoundException($"El attributo {entity.Id}-{entity.Field} no encontrado para el grupo {parent.Id}-{parent.Name}.");
        }

        public async override Task<bool> HasChanges(Property entity, Property persited)
        {
            return entity.Id != persited.Id
                || string.Compare(entity.Field, persited.Field, true) != 0
                || string.Compare(entity.Value, persited.Value, true) != 0;
        }
    }
}
