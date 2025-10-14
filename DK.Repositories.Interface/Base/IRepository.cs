using Dakali.Interface;
using System.Threading.Tasks;

namespace DK.Repositories.Interface.Base
{
    public interface IRepository<T> where T : IEntity
    {
        Task<T> Get(long id);
        Task<T> Create(T entity);
        Task<T> Update(T entity);
        Task Delete(T entity);
    }
}
