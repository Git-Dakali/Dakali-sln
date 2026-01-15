// See https://aka.ms/new-console-template for more information
using Dakali.Domine.Connection;
using Dakali.Interface.Connection;
using DK.DatabaseMigrations;
using DK.Process;
using DK.Repositories;
using DK.Validator;
using ICR.DatabaseMigrations.Deployments._1_0_0;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

IHost host = Host.CreateDefaultBuilder(args)
     .UseWindowsService(options =>
     {
         options.ServiceName = "DK.DataBaseMigrations";

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
        DependencyInjectionProcess.Configure(services);
        DependencyInjectionValidator.Configure(services);
        DependencyInjectionRepository.Configure(services);
        services.AddScoped<Release_1_0_0>();
        #endregion

    })
    .Build();
await host.RunAsync();