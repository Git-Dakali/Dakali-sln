using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dakali.Interface
{
    public interface ISession
    {
        SqlConnection Connection { get; set; }
        SqlTransaction Transaction { get; set; }

        SqlConnection GetConnection();

        Task Commit();

        Task Rollback();
    }
}
