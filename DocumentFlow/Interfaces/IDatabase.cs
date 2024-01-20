//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;

using System.Data;

namespace DocumentFlow.Interfaces;

public interface IDatabase
{
    string ConnectionName { get; }
    string ConnectionString { get; }
    IDbConnection OpenConnection();
    bool HasPrivilege(string tableName, params Privilege[] privilege);
    IDatabase WithName(string name);
    IDatabase WithServer(string server, int port);
    IDatabase WithDatabase(string database);
    IDatabase WithCredentials(string userName, string password);
    void Build();
}
