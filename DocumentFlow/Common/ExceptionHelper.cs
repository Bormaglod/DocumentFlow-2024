//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;

using Npgsql;

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DocumentFlow.Common;

public static class ExceptionHelper
{
    private class ExceptionData
    {
        [JsonPropertyName("table")]
        public string TableName { get; set; } = string.Empty;

        [JsonPropertyName("trigger")]
        public string TriggerName { get; set; } = string.Empty;

        [JsonPropertyName("function_name")]
        public string FunctionName { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    public static string Message(Exception exception, IDatabase? database = null)
    {
        StringBuilder stringBuilder = new();
        CreateMessage(stringBuilder, exception, database);
        return stringBuilder.ToString();
    }

    private static void CreateMessage(StringBuilder strings, Exception exception, IDatabase? database = null)
    {
        if (exception is PostgresException pgException)
        {
            if (pgException.SqlState == "P0001")
            {
                if (pgException.MessageText.IsJson())
                {
                    var message = JsonSerializer.Deserialize<ExceptionData>(pgException.MessageText);
                    if (message != null)
                    {
                        strings.AppendLine(message.Text);
                    }
                    else
                    {
                        strings.AppendLine(pgException.MessageText);
                    }
                }
                else
                {
                    strings.AppendLine(pgException.MessageText);
                }
            }
            else if (pgException.SqlState == "23514")
            {
                string? msg = string.Empty;

                if (database != null)
                {
                    using var conn = database.OpenConnection();
                    msg = conn.QuerySingleOrDefault<string>($"select d.description from pg_catalog.pg_constraint c join pg_catalog.pg_description d on (d.objoid = c.oid) where conname = '{pgException.ConstraintName}'");
                }

                if (string.IsNullOrEmpty(msg))
                {
                    strings.AppendLine(DefaultMessage(pgException));
                }
                else
                {
                    strings.AppendLine(msg);
                }
            }
            else
            {
                strings.AppendLine(DefaultMessage(pgException));
            }
        }
        else if (exception is AggregateException aggregateException)
        {
            foreach (Exception e in aggregateException.InnerExceptions)
            {
                CreateMessage(strings, e);
            }
        }
        else
        {
            strings.AppendLine(exception.Message);
            if (exception.InnerException != null)
            {
                CreateMessage(strings, exception.InnerException);
            }
        }
    }

    private static string DefaultMessage(PostgresException exception)
    {
        // TODO: Из PostgresException удалено свойство NpgsqlStatement? Statement. Чем заменить?
        return string.Format("{0}\nSQL: {1}", exception.MessageText, exception.Detail);
    }
}
