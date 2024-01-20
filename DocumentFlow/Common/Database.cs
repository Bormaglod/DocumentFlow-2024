//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces;

using Microsoft.Extensions.Logging;

using NLog.Extensions.Logging;

using Npgsql;

using System.Data;
using System.Windows;

namespace DocumentFlow.Common;

public class Database : IDatabase
{
    private NpgsqlDataSource? dataSource;

    private string name = string.Empty;
    private string server = "localhost";
    private int port = 5432;
    private string databaseName = string.Empty;
    private string userName = "guest";
    private string password = "guest";

    public string ConnectionName => name;

    public string ConnectionString { get; private set; } = string.Empty;

    public IDbConnection OpenConnection()
    {
        if (string.IsNullOrEmpty(ConnectionString))
        {
            ArgumentException.ThrowIfNullOrEmpty(ConnectionString);
        }

        if (dataSource == null)
        {
            ArgumentNullException.ThrowIfNull(dataSource);
        }

        while (true)
        {
            try
            {
                return dataSource.OpenConnection();
            }
            catch (NpgsqlException e)
            {
                if (e.InnerException is TimeoutException)
                {
                    if (MessageBox.Show("Сервер не отвечает. Повторить?", "Ошибка", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }

            }
        }
    }

    public bool HasPrivilege(string tableName, params Privilege[] privilege)
    {
        using var conn = OpenConnection();
        if (privilege.Length == 0)
        {
            return false;
        }

        return conn.ExecuteScalar<bool>("select has_table_privilege(:user, :table, :privilege)", new
        {
            user = userName,
            table = tableName,
            privilege = string.Join(',', privilege)
        });
    }

    public IDatabase WithName(string name)
    {
        this.name = name;
        return this;
    }

    public IDatabase WithServer(string server, int port)
    {
        this.server = server;
        this.port = port;
        return this;
    }

    public IDatabase WithDatabase(string database)
    {
        databaseName = database;
        return this;
    }

    public IDatabase WithCredentials(string userName, string password)
    {
        this.userName = userName;
        this.password = password;
        return this;
    }

    public void Build()
    {
        var builder = new NpgsqlConnectionStringBuilder()
        {
            Host = server,
            Port = port,
            Database = databaseName,
            Username = userName,
            Password = password
        };

        ConnectionString = builder.ConnectionString;

        var loggerFactory = LoggerFactory.Create(builder => builder.AddNLog());
        NpgsqlLoggingConfiguration.InitializeLogging(loggerFactory, parameterLoggingEnabled: true);

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString);
        dataSourceBuilder
            .UseLoggerFactory(loggerFactory)
            .EnableParameterLogging();
        dataSource = dataSourceBuilder.Build();
    }
}
