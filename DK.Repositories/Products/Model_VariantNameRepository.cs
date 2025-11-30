using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class Model_VariantNameRepository : RepositoryReferencePrimitive<Model, string>
    {
        private readonly ISession _session;

        public Model_VariantNameRepository(ISession session)
        {
            _session = session;
        }

        private async Task Create(Model parent, string value, CancellationToken cancellation = default)
        {
            var sql = @"
                INSERT INTO dbo.Model_VariantName (ModelId, Name) 
                OUTPUT INSERTED.Name
                VALUES(@ModelId, @Name);";

            await _session.Connection.QuerySingleAsync<string>(new CommandDefinition(sql, new { ModelId = parent.Id, Name = value }, _session.Transaction, cancellationToken: cancellation));
        }

        public override async Task<IEnumerable<string>> Create(Model parent, IEnumerable<string> values, CancellationToken cancellation = default)
        {
            foreach (var value in values)
                await Create(parent, value, cancellation);

            return await Get(parent, cancellation);
        }

        public override async Task Delete(Model parent, CancellationToken cancellation = default)
        {
            var sql = @"
                DELETE dbo.Model_VariantName
                WHERE ModelId = @ModelId;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(sql, new { ModelId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public override async Task<IEnumerable<string>> Get(Model parent, CancellationToken cancellation = default)
        {
            var sql = @"SELECT Name FROM dbo.Model_VariantName WHERE ModelId = @ModelId;";

            return await _session.Connection.QueryAsync<string>(new CommandDefinition(sql, new { ModelId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }
    }
}
