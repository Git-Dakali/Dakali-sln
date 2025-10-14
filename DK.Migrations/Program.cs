// See https://aka.ms/new-console-template for more information
using Dakali.Domine.Connection;
using Dakali.Interface.Connection;
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

        //USER DATA
        services.AddSingleton<ServiceDataBaseMigration>();
        services.AddHostedService<ServiceDataBaseMigration>();
        services.AddScoped<IConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<ISession, Session>();

        #region Dependency Injection Runners

        services.AddScoped(typeof(Release_1_0_0));
        #endregion

    })
    .Build();
await host.RunAsync();