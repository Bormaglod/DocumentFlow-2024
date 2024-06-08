//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;

using Humanizer;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Reflection;

namespace DocumentFlow.ViewModels.Browsers;

public abstract partial class BalanceViewModel<T> : DocumentViewModel<T>
    where T : Balance
{
    public BalanceViewModel() { }

    public BalanceViewModel(IDatabase database, IConfiguration configuration, ILogger<BalanceViewModel<T>> logger) : base(database, configuration, logger) { }

    [RelayCommand]
    private void OpenDocument(object parameter)
    {
        if (parameter is T balance && balance.DocumentCode != null)
        {
            var viewType = $"{balance.DocumentCode.Pascalize()}View";
            var type = Assembly
                .GetExecutingAssembly()
                .DefinedTypes
                .FirstOrDefault(x => x.Name == viewType && x.Namespace != null && x.Namespace.StartsWith("DocumentFlow.Views.Editors"));

            if (type != null)
            {
                WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(type, balance.ReferenceId));
            }
        }
    }

    protected override void InitializeToolBar()
    {
        ToolBarItems.AddButtons(this,
            new ToolBarButtonModel("Открыть документ", "open-document") { Command = OpenDocumentCommand },
            new ToolBarSeparatorModel(),
            new ToolBarButtonComboModel("Печать", "print"),
            new ToolBarButtonModel("Настройки", "settings"));
    }
}
