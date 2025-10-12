using Dakali.Interface;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace Dakali
{
    public class Session : ISession
    {
        public SqlConnection Connection { get; set; }
        public SqlTransaction Transaction { get; set; }

        public Session(string connectionString, bool openTransaction = false)
        {
            Connection = new SqlConnection(connectionString);
            Connection.Open();

            if (openTransaction)
                Transaction = Connection.BeginTransaction();
        }

        public SqlConnection GetConnection()
        {
            if (Connection == null)
                throw new Exception("No existe una conexion abierta");
            if (Connection.State != System.Data.ConnectionState.Open)
                throw new Exception($"La conexion se encuentra en estado {Connection.State.ToString()}");
            return Connection;
        }

        public async Task Commit()
        {
            if (Connection == null)
                throw new Exception("No existe una conexion abierta");

            if (Transaction != null)
            {
                await Transaction.CommitAsync();
                await Transaction.DisposeAsync();
                Transaction = null;
            }

            await Connection.CloseAsync();
            await Connection.DisposeAsync();
            Connection = null;
        }

        public async Task Rollback()
        {
            if (Connection == null)
                throw new Exception("No existe una conexion abierta");

            if (Transaction != null)
            {
                await Transaction.RollbackAsync();
                await Transaction.DisposeAsync();
                Transaction = null;
            }

            await Connection.CloseAsync();
            await Connection.DisposeAsync();
            Connection = null;
        }
    }
}
