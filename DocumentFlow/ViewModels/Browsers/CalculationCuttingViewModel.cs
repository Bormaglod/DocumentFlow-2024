//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class CalculationCuttingViewModel : BaseCalculationOperationViewModel<CalculationCutting>, ISelfTransientLifetime
{
    public CalculationCuttingViewModel() { }

    public CalculationCuttingViewModel(IDatabase database, ICalculationCuttingRepository repoOperations) : base(database, repoOperations) { }

    public override Type? GetEditorViewType() => typeof(Views.Editors.CalculationCuttingView);
}