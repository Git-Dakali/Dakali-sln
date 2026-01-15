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

        protected async Task<int> ExecuteCommand(string sql, object? param = null, int? timeoutSeconds = null)
        {
            var cmd = new CommandDefinition(
                commandText: sql,
                parameters: param,
                transaction: _session.Transaction,               // ← IMPORTANTE
                commandTimeout: timeoutSeconds);

            return await _session.Connection.ExecuteAsync(cmd);
        }

        // si tenías un método que recorre SQLs:
        protected async Task RunSqlStatements()
        {
            foreach (var sql in SQLs)
            {
                if (string.IsNullOrWhiteSpace(sql)) 
                    continue;
                await ExecuteCommand(sql);
            }
        }

        public abstract Task Run();
    }
}
