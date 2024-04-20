//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;
using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public abstract class DirectoryRepository<T> : ReadOnlyRepository<T>, IDirectoryRepository<T>
    where T : Directory
{
    public DirectoryRepository(IDatabase database) : base(database) { }

    public IReadOnlyList<T> GetSlim(DocumentInfo owner)
    {
        using var conn = GetConnection();
        return GetSlim(conn, owner);
    }

    public IReadOnlyList<T> GetSlim(IDbConnection connection, DocumentInfo owner) => GetSlimQuery(connection, owner).Get<T>().ToList();

    protected override Query GetSlimQuery(IDbConnection connection) => GetSlimQuery(connection, null);

    protected virtual Query GetSlimQuery(IDbConnection connection, DocumentInfo? owner = null)
    {
        return GetCustomSlimQuery<T>(connection, owner);
    }

    protected Query GetCustomSlimQuery<P>(IDbConnection connection, DocumentInfo? owner = null)
        where P : Directory
    {
        return connection.GetQuery<P>(GetDefaultSlimQueryParameters())
            .WhereFalse("t0.deleted")
            .When(owner != null, q => q.Where("t0.owner_id", owner!.Id))
            .OrderBy("t0.item_name")
            .Select("t0.{parent_id,is_folder}");
    }

    protected override QueryParameters GetDefaultSlimQueryParameters()
    {
        return new QueryParameters()
        {
            IncludeDocumentsInfo = false,
            Quantity = QuantityInformation.DirectoryExt
        };
    }
}
