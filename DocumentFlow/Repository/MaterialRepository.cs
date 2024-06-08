//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public class MaterialRepository(IDatabase database) : 
    ProductRepository<Material>(database), 
    IMaterialRepository, 
    ITransientLifetime
{
    public IReadOnlyList<Material> GetMaterials()
    {
        using var conn = GetConnection();
        return GetMaterials(conn);
    }

    public IReadOnlyList<Material> GetMaterials(IDbConnection connection)
    {
        string sql = @"with recursive r as
                       (
                           select * from material where material_kind != 'wire'
                           union
	                       select p.* from material p join r on (r.parent_id = p.id)
                       )
                       select * from r where not deleted order by item_name";

        return connection.Query<Material>(sql).ToList();
    }

    public IReadOnlyList<Material> GetWires()
    {
        using var conn = GetConnection();
        return GetWires(conn);
    }

    public IReadOnlyList<Material> GetWires(IDbConnection connection)
    {
        string sql = @"with recursive r as
                       (
                           select * from material where material_kind = 'wire'
                           union
	                       select p.* from material p join r on (r.parent_id = p.id)
                       )
                       select * from r where not deleted order by item_name";

        return connection.Query<Material>(sql).ToList();
    }

    public IReadOnlyList<Material> GetCrossMaterials()
    {
        using var conn = GetConnection();
        return GetCrossMaterials(conn);
    }

    public IReadOnlyList<Material> GetCrossMaterials(IDbConnection connection) => GetCrossMaterials(connection, Guid.Empty);

    public IReadOnlyList<Material> GetCrossMaterials(Guid exceptMaterial)
    {
        using var conn = GetConnection();
        return GetCrossMaterials(conn, exceptMaterial);
    }

    public IReadOnlyList<Material> GetCrossMaterials(IDbConnection connection, Guid exceptMaterial)
    {
        return GetSlimQuery(connection)
            .WhereNull("owner_id")
            .When(exceptMaterial != Guid.Empty, q => q.WhereNot("id", exceptMaterial))
            .Get<Material>()
            .ToList();
    }

    public IList<CompatiblePart> GetCompatibleParts(Material material)
    {
        using var conn = GetConnection();
        return GetCompatibleParts(conn, material);
    }

    public IList<CompatiblePart> GetCompatibleParts(IDbConnection connection, Material material)
    {
        var sql = "select cp.*, m.id, m.code, m.item_name from compatible_part as cp join material m on m.id = cp.compatible_id where cp.owner_id = :Id";
        return connection.Query<CompatiblePart, Material, CompatiblePart>(sql, (cp, material) =>
        {
            cp.Compatible = material;
            return cp;
        }, material).ToList();
    }
}
