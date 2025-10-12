using System;
using System.Collections.Generic;
using System.Text;

namespace Dakali.Interface
{
    public interface IContextManager
    {
        Session Session { get; set; }
        Session OpenSession(string connectionString, bool openTransaction = false);
        Session NewOpenSession(string connectionString, bool openTransaction = false);
    }
}
