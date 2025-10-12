namespace Dakali
{
    public class ContextManager
    {
        public static Session Session { get; set; }

        public static Session OpenSession(string connectionString, bool openTransaction = false)
        {
            Session = NewOpenSession(connectionString, openTransaction);
            return Session;
        }

        public static Session NewOpenSession(string connectionString, bool openTransaction = false)
        {
            return new Session(connectionString, openTransaction);
        }
    }
}
