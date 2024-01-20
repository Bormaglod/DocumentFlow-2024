//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Common.Data;
using DocumentFlow.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DocumentFlow.Models.Settings;
using NLog.Extensions.Logging;

using Syncfusion.SfSkinManager;

using System.IO;
using System.Windows;
using DocumentFlow.Models.Settings.Authentification;
using Minio;

namespace DocumentFlow;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost host;

    public App()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        Dapper.SqlMapper.AddTypeHandler(new DapperSqlDateOnlyTypeHandler());
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        FastReport.Utils.RegisteredObjects.AddConnection(typeof(FastReport.Data.PostgresDataConnection));

        ToastOperations.OnActivated();

        var localSettings = Path.Combine(
#if !DEBUG
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Автоком",
        "settings",
#endif
"appsettings.local.json"
);

        var browserSettings = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Автоком",
            "settings",
            "browsers"
        );

        SfSkinManager.ApplyStylesOnApplication = true;

        host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.auth.json");
                builder.AddJsonFile(localSettings, optional: true);
                if (Directory.Exists(browserSettings))
                {
                    foreach (var item in Directory.GetFiles(browserSettings, "*.json"))
                    {
                        builder.AddJsonFile(item, optional: true);
                    }
                }
            })
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(context.Configuration, services);
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddNLog();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await host.StartAsync();

        ServiceLocator.Initialize(host.Services.GetRequiredService<IDependencyContext>());

        var mainWindow = host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await host.StopAsync();
        base.OnExit(e);
    }

    private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.Configure<LocalSettings>(configuration.GetSection("LocalSettings"));

        services.AddAdvancedDependencyInjection();

        services.AddMinio(configureClient =>
        {
            var auth = new MinioAuth();
            configuration.GetSection("Authentification:Minio").Bind(auth);
            configureClient
                .WithEndpoint(auth.ToString())
                .WithCredentials(auth.AccessKey, auth.SecretKey)
                .WithSSL(false);
        });

        services.AddDatabase(configure =>
        {
            var connections = configuration.GetSection("AppSettings:Connections").Get<List<ConnectionSettings>>();
            if (connections == null) throw new NullReferenceException(nameof(connections));

            var connectionName = configuration.GetSection("LocalSettings:ConnectionName").Get<string>();
            if (connectionName == null) throw new NullReferenceException(nameof(connectionName));

            var connection = connections.FirstOrDefault(x => x.Name == connectionName);
            if (connection == null) throw new NullReferenceException(nameof(connection));

            var auth = configuration.GetSection("Authentification:Postgresql").Get<PostgresqlAuth>();
            if (auth == null) throw new NullReferenceException(nameof(auth));

            configure
                .WithName(connectionName)
                .WithDatabase(connection.Database)
                .WithServer(connection.Server, connection.Port)
                .WithCredentials(auth.Login, auth.Password);
        });
    }

    // https://stackoverflow.com/questions/543414/app-xaml-file-does-not-get-parsed-if-my-app-does-not-set-a-startupuri/555286#comment90714334_555286
    private void Application_Startup(object sender, StartupEventArgs e)
    {

    }
}
