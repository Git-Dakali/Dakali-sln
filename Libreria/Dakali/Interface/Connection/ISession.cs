using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Dakali.Interface.Connection
{
    public interface ISession: IDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; }
        Task BeginTransaction(CancellationToken ct = default);
        Task Commit(CancellationToken ct = default);
        Task Rollback(CancellationToken ct = default);
    }
}
