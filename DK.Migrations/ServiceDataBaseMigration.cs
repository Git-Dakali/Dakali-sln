using Dakali;
using ICR.DatabaseMigrations.Deployments._1_0_0;
using Microsoft.Extensions.Configuration;
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
            var configuration = _serviceProvider.GetService<IConfiguration>();
            var logger = _serviceProvider.GetService<ILogger<ServiceDataBaseMigration>>();

            logger.LogInformation("Starting database migration...");
            ContextManager.OpenSession(configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value ?? string.Empty);

            try {
                new Release_1_0_0().Run();
                await ContextManager.Session.Commit();

                logger.LogInformation("Database migration completed successfully.");

            }
            catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred.");
                await ContextManager.Session.Rollback();

                throw;
            }
            
            await Task.CompletedTask;
        }
    }
}
