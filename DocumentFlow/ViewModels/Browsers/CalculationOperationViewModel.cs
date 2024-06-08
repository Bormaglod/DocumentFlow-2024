//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class CalculationOperationViewModel : BaseCalculationOperationViewModel<CalculationOperation>, ISelfTransientLifetime
{
    public CalculationOperationViewModel() { }

    public CalculationOperationViewModel(IDatabase database, ICalculationOperationRepository repoOperations, IConfiguration configuration, ILogger<CalculationOperationViewModel> logger) 
        : base(database, repoOperations, configuration, logger) 
    { 
    }

    public override Type? GetEditorViewType() => typeof(Views.Editors.CalculationOperationView);
}
