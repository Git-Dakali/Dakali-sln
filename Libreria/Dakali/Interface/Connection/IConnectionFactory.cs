using System.Data;

namespace Dakali.Interface.Connection
{
    public interface IConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
