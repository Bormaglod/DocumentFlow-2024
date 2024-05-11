//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class CalculationViewModel : DirectoryViewModel<Calculation>, ISelfTransientLifetime
{
    public CalculationViewModel() { }

    public CalculationViewModel(IDatabase database, IConfiguration configuration) : base(database, configuration) { }

    public override Type? GetEditorViewType() => typeof(Views.Editors.CalculationView);

    protected override bool CanEditSelected(Calculation selectedItem)
    {
        return base.CanEditSelected(selectedItem) && selectedItem.CalculationState != Common.Enums.CalculationState.Expired;
    }

    protected override void RegisterReports()
    {
        base.RegisterReports();
        RegisterReport(Report.Specification);
        RegisterReport(Report.ProcessMap);
    }
}
