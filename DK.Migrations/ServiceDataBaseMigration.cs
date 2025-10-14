using ICR.DatabaseMigrations.Deployments._1_0_0;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DK.DatabaseMigrations
{
    public class ServiceDataBaseMigration : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceDataBaseMigration(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Implement the logic to run the database migration here
            var logger = _serviceProvider.GetService<ILogger<ServiceDataBaseMigration>>();
            var session = _serviceProvider.GetRequiredService<Dakali.Interface.Connection.ISession>();
            
            logger.LogInformation("Starting database migration...");
            await session.BeginTransaction();

            try
            {
                new Release_1_0_0(session).Run();
                await session.Commit();

                logger.LogInformation("Database migration completed successfully.");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred.");
                await session.Rollback();

                throw;
            }
            finally
            {
                (session as IDisposable)?.Dispose();
            }
            
            await Task.CompletedTask;
        }
    }
}
