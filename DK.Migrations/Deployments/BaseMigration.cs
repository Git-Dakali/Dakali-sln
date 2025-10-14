using Dakali.Interface.Connection;
using Dapper;

namespace DK.DatabaseMigrations.Deployments
{
    public abstract class BaseMigration
    {
        private ISession _session;
        public virtual List<string> SQLs { get; set; }

        public BaseMigration(ISession session)
        {
            SQLs = new List<string>();
            _session = session;
        }

        protected int ExecuteCommand(string sql, object? param = null, int? timeoutSeconds = null)
        {
            var cmd = new CommandDefinition(
                commandText: sql,
                parameters: param,
                transaction: _session.Transaction,               // ← IMPORTANTE
                commandTimeout: timeoutSeconds);

            return _session.Connection.Execute(cmd);
        }

        // si tenías un método que recorre SQLs:
        protected void RunSqlStatements()
        {
            foreach (var sql in SQLs)
            {
                if (string.IsNullOrWhiteSpace(sql)) continue;
                ExecuteCommand(sql);
            }
        }

        public abstract void Run();
    }
}
