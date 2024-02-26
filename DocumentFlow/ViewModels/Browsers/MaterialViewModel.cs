//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public class MaterialViewModel : ProductViewModel<Material>, ISelfTransientLifetime
{
    public MaterialViewModel() { }

    public MaterialViewModel(IDatabase database) : base(database) { }

    public override Type? GetEditorViewType() => typeof(Views.Editors.MaterialView);

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<Material>(x => x.Measurement)
            .MappingQuery<Material>(x => x.Wire)
            .MappingQuery<Material>(x => x.Cross);
    }

    protected override IReadOnlyList<Material> GetData(IDbConnection connection, Guid? id)
    {
        var parameters = new QueryParemeters()
        {
            Table = "material_ext"
        };

        return DefaultQuery(connection, id)
            .Get<Material, Measurement, Wire, Material>(
                map: (material, measurement, wire, cross) =>
                {
                    material.Measurement = measurement;
                    material.Wire = wire;
                    material.Cross = cross;
                    return material;
                })
            .ToList();
    }
}
