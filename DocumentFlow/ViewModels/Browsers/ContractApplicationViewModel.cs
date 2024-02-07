//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Browsers;

public class ContractApplicationViewModel : DirectoryViewModel<ContractApplication>, ISelfTransientLifetime
{
    public ContractApplicationViewModel() { }

    public ContractApplicationViewModel(IDatabase database) : base(database) { }

    public override Type? GetEditorViewType() => typeof(Views.Editors.ContractApplicationView);
}
