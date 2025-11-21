using Dakali.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Interface.Base
{
    public interface IRepositoryCode<T> : IRepository<T> where T : IEntity
    {
        Task<T> Get(string code, CancellationToken cancellation = default);
    }
}
