//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public class CuttingRepository(IDatabase database) : 
    DirectoryRepository<Cutting>(database), 
    ICuttingRepository, 
    ITransientLifetime
{
    public IReadOnlyList<Cutting> GetOperations()
    {
        using var conn = GetConnection();
        return GetOperations(conn);
    }

    public IReadOnlyList<Cutting> GetOperations(IDbConnection connection)
    {
        var parameters = new QueryParameters()
        {
            IncludeDocumentsInfo = false,
            Quantity = QuantityInformation.DirectoryExt
        };

        return connection.GetQuery<Cutting>(parameters)
            .WhereFalse("deleted")
            .OrderBy("item_name")
            .Select("parent_id", "is_folder", "salary", "segment_length")
            .Get<Cutting>()
            .ToList();
    }

    public IReadOnlyList<int> GetAvailableProgram(Cutting? cutting)
    {
        using var conn = GetConnection();
        return GetAvailableProgram(conn, cutting);
    }

    public IReadOnlyList<int> GetAvailableProgram(IDbConnection connection, Cutting? cutting)
    {
        if (cutting != null && cutting.ProgramNumber.HasValue)
        {
            var sql = $"with all_programs as ( select generate_series(1, 99) as id ) select a.id from all_programs a left join cutting on (program_number = a.id and not deleted) where program_number is null or program_number = :ProgramNumber order by a.id";
            return connection.Query<int>(sql, cutting).ToList();
        }
        else
        {
            var sql = $"with all_programs as ( select generate_series(1, 99) as id ) select a.id from all_programs a left join cutting on (program_number = a.id and not deleted) where program_number is null order by a.id";
            return connection.Query<int>(sql).ToList();
        }
    }
}