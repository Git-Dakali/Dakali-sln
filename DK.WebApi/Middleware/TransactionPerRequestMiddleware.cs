namespace DK.WebApi.Middleware
{
    public class TransactionPerRequestMiddleware
    {
        private readonly RequestDelegate _next;
        public TransactionPerRequestMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            // Obtenemos la Session SCOPED del request
            var session = context.RequestServices.GetRequiredService<Dakali.Interface.Connection.ISession>();
            await session.BeginTransaction(context.RequestAborted);

            try
            {
                await _next(context);

                // Si es GET (o HEAD) => rollback forzado
                var method = context.Request.Method.ToUpperInvariant();
                if (method == HttpMethods.Get || method == HttpMethods.Head)
                {
                    await session.Rollback(context.RequestAborted);
                    return;
                }

                // Para otros métodos: commit si el status fue exitoso (<400)
                if (context.Response.StatusCode < 400)
                    await session.Commit(context.RequestAborted);
                else
                    await session.Rollback(context.RequestAborted);
            }
            catch
            {
                await session.Rollback(context.RequestAborted);
                throw;
            }
            finally
            {
                (session as IDisposable)?.Dispose();
            }
        }
    }
}
