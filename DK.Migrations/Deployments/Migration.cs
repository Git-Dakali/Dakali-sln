using Dakali.Interface.Connection;

namespace DK.DatabaseMigrations.Deployments
{
    public abstract class Migration : BaseMigration
    {
        public virtual string Summary { get; set; }

        public Migration(ISession session)
            : base(session)
        {

        }

        public Migration(ISession session, string sumary)
            : this(session)
        {
            Summary = sumary;
        }

        public abstract Task BasicRun();

        public async override Task Run()
        {
            //CTX.CM.Log().Info("Running JIRA Issue [" + GetType().Name.Replace("__", "/").Replace('_', '-') + "]" + Summary);
            //if (CTX.CM.Session().ExistsCode<JiraIssueMigrationExecution>(Execution.Code))
            //throw new Exception($"Jira Issue Already Runned [{Execution.Code}] {Execution.Summary}");
            //Execution.Start = DateTime.Now;
            await RunSqlStatements();
            await BasicRun();
            //Execution.Finish = DateTime.Now;
            //CTX.CM.Session().Add(Execution);
            //CTX.CM.Log().Info("Running JIRA Issue [" + GetType().Name.Replace("__", "/").Replace('_', '-') + "]" + Summary + " succesfully executed");
            //CTX.CM.Log().Info("");
        }
    }
}
