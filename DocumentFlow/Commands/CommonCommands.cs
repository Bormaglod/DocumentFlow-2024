//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common;
using DocumentFlow.Common.Enums;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;
using DocumentFlow.Models.Settings;
using DocumentFlow.Tools;

using Microsoft.Extensions.Options;

using Syncfusion.Windows.Shared;

using System.Windows.Controls;

namespace DocumentFlow.Commands;

public static class CommonCommands
{
    #region OpenReportCommand

    static RelayCommand<SelectionChangedEventArgs>? openReportCommand;

    public static IRelayCommand<SelectionChangedEventArgs> OpenReportCommand => openReportCommand ??= new RelayCommand<SelectionChangedEventArgs>(OpenReport);

    private static void OpenReport(SelectionChangedEventArgs? e)
    {
        if (e == null)
        {
            return;
        }

        if (e.Source is ListBox list)
        {
            list.SelectedIndex = -1;
        }

        if (e.AddedItems.Count > 0 &&
            e.AddedItems[0] is MenuItemModel model)
        {
            if (model.Tag is not Report report)
            {
                throw new Exception("Свойство Tag в объекте класса MenuItemModel должно иметь тип Models.Entities.Report");
            }

            if (model.PlacementTarget is not IReport r)
            {
                throw new Exception("Свойство PlacementTarget в объекте класса MenuItemModel должно реализовывать интерфейс IReport");
            }

            var document = r.GetReportingDocument(report);
            if (document == null || string.IsNullOrEmpty(report.SchemaReport))
            {
                return;
            }

            FastReport.Report fastReport = new();

            string header = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            fastReport.LoadFromString(header + report.SchemaReport);
            fastReport.SetParameterValue("id", document.Id.ToString());

            var conn = fastReport.Dictionary.Connections[0];
            conn.ConnectionString = ServiceLocator.Context.GetService<IDatabase>().ConnectionString;

            fastReport.Prepare();

            var ls = ServiceLocator.Context.GetService<IOptions<LocalSettings>>().Value;
            string file = PdfHelper.CreateDocument(fastReport, ls.Report.Resolution, PdfNamingStrategy.Guid);

            var window = new PreviewReportWindow();
            WeakReferenceMessenger.Default.Send(new OpenPdfReportMessage() { PdfFile = file, Document = document });
            window.Show();
        }
    }

    #endregion

    #region ClearTextValue

    static RelayCommand<object>? clearTextValueCommand;

    public static IRelayCommand<object> ClearTextValueCommand => clearTextValueCommand ??= new RelayCommand<object>(ClearTextValue);

    private static void ClearTextValue(object? parameter)
    {
        switch (parameter)
        {
            case CurrencyTextBox currency: 
                currency.Value = null;
                break;
        }
    }

    #endregion
}
