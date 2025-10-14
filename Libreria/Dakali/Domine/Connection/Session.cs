using Dakali.Interface.Connection;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Dakali.Domine.Connection
{
    public class Session : ISession
    {
        public IDbConnection Connection { get; private set; }
        public IDbTransaction? Transaction { get; private set; }

        public Session(IConnectionFactory factory)
        {
            Connection = factory.CreateConnection();
        }

        public async Task BeginTransaction(CancellationToken cancellationToken = default)
        {
            if (Connection.State != ConnectionState.Open)
            {
                if (Connection is System.Data.Common.DbConnection dbc)
                    await dbc.OpenAsync(cancellationToken);
                else
                    Connection.Open();
            }

            Transaction = Connection.BeginTransaction();
        }

        public Task Commit(CancellationToken ct = default)
        {
            Transaction?.Commit();
            return Task.CompletedTask;
        }

        public Task Rollback(CancellationToken ct = default)
        {
            if (Transaction != null)
            {
                try { Transaction.Rollback(); } catch { /* swallow if already completed */ }
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            if (Connection.State != ConnectionState.Closed) Connection.Close();
            Connection.Dispose();
        }
    }
}
