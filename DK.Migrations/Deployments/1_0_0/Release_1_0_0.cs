using DK.DatabaseMigrations.Deployments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICR.DatabaseMigrations.Deployments._1_0_0
{
    public class Release_1_0_0 : Migration
    {
        public Release_1_0_0 () {
            SQLs.Add(@"
                CREATE TABLE WebHookEvents (
                    Id bigint identity(1, 1),
                    EventType varchar(255),
                    JSon text,
                    IsProcessed bit,
                    Error varchar(500)
                );
            ");
        }
        public override void BasicRun()
        {
        }
    }
}
