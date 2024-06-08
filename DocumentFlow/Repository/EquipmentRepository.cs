//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public class EquipmentRepository(IDatabase database) : 
    DirectoryRepository<Equipment>(database), 
    IEquipmentRepository, 
    ITransientLifetime
{
    public IReadOnlyList<Equipment> GetEquipments()
    {
        using var conn = GetConnection();
        return GetEquipments(conn);
    }

    public IReadOnlyList<Equipment> GetEquipments(IDbConnection connection)
    {
        return GetSlimQuery(connection)
            .WhereFalse("is_tools")
            .Get<Equipment>()
            .ToList();
    }

    public IReadOnlyList<Equipment> GetTools()
    {
        using var conn = GetConnection();
        return GetTools(conn);
    }

    public IReadOnlyList<Equipment> GetTools(IDbConnection connection)
    {
        return GetSlimQuery(connection)
            .WhereTrue("is_tools")
            .Get<Equipment>()
            .ToList();
    }
}
