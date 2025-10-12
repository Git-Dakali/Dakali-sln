using Dakali;
using Microsoft.Data.SqlClient;

namespace DK.DatabaseMigrations.Deployments
{
    public abstract class BaseMigration
    {
        public virtual List<string> SQLs { get; set; }

        public BaseMigration()
        {
            SQLs = new List<string>();
        }

        public virtual void RunSqlStatements(int maxCommandTimeout = 30)
        {
            foreach (var sql in SQLs)
                ExecuteCommand(sql, timeout: maxCommandTimeout);
        }

        public virtual void ExecuteCommand(string sql, string name = null, int timeout = 30)
        {
            using (var command = new SqlCommand(sql))
            {
                command.Connection = ContextManager.Session.Connection;
                ExecuteCommand(command, name, timeout);
            }
        }

        public virtual void ExecuteCommand(SqlCommand command, string name = null, int timeout = 30)
        {
            if (name != null)
            {
                //CTX.CM.Log().Info(name);
                //CTX.CM.Log().Debug('\t' + command.CommandText.Replace("\n", "\n\t"));
            }
            else
            {
                //CTX.CM.Log().Debug('\t' + command.CommandText.Replace("\n", "\n\t"));
            }
            command.CommandTimeout = timeout;
            //CTX.CM.Session().Flush();
            //CTX.CM.Session().Enlist(command);
            var start = DateTime.Now;
            var task = command.BeginExecuteNonQuery();
            var writed = false;
            while (!task.IsCompleted)
            {
                var elapsed = DateTime.Now - start;
                var remaining = TimeSpan.FromSeconds(timeout) - elapsed;
                if (elapsed.TotalSeconds >= 1)
                {
                    writed = true;
                    Console.Write("\r  - {0:mm\\:ss} running. {1:mm\\:ss} until timeout", elapsed, remaining);
                }
                Thread.Sleep(elapsed.TotalSeconds < 2 ? 100 : 1000);
            }

            var rows = command.EndExecuteNonQuery(task);
            if (writed)
                Console.Write("\r                                     ");
            //CTX.CM.Log().Debug("  - {0} rows affected in {1:mm\\:ss\\.ffff}", rows, DateTime.Now - start);
        }

        public abstract void Run();
    }
}
