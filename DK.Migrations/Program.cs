// See https://aka.ms/new-console-template for more information
using DK.DatabaseMigrations;
using ICR.DatabaseMigrations.Deployments._1_0_0;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

IHost host = Host.CreateDefaultBuilder(args)
     .UseWindowsService(options =>
     {
         options.ServiceName = "ICR.DataBaseMigrations";

     })
    .ConfigureServices((hostContext, services) =>
    {
        LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(services);

        #region Services for Hibernate

        //interceptor = services.AddNHibernate(hostContext.Configuration["ConnectionStrings:DefaultConnection"]) as DependencyInjectionEntityInterceptor;
        #endregion

        //USER DATA
        services.AddSingleton<ServiceDataBaseMigration>();
        services.AddHostedService<ServiceDataBaseMigration>();

        #region Dependency Injection Runners
        
        services.AddScoped(typeof(Release_1_0_0));
        #endregion

    })
    .Build();
await host.RunAsync();