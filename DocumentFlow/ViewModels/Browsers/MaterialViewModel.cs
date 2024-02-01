//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

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
        return base.SelectQuery(query).From("material_ext as t0");
    }

    protected override IReadOnlyList<Material> GetData(IDbConnection connection, Guid? id)
    {
        return GetData<Measurement, Wire, Material>(connection, (material, measurement, wire, cross) => 
        { 
            material.Measurement = measurement;
            material.Wire = wire;
            material.Cross = cross;
            return material; 
        },
        refs =>
        {
            return refs switch
            {
                "material_id" => "owner_id",
                _ => refs
            };
        },
        id: id);
    }
}
