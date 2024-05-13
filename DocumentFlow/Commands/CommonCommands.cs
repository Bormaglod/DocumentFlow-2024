//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

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
using System.Windows.Input;

namespace DocumentFlow.Commands;

public static class CommonCommands
{
    #region OpenReport

    static ICommand? openReport;

    public static ICommand OpenReport
    {
        get
        {
            openReport ??= new DelegateCommand<SelectionChangedEventArgs>(OnOpenReport);
            return openReport;
        }
    }

    private static void OnOpenReport(SelectionChangedEventArgs e)
    {
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

    static ICommand? clearTextValue;

    public static ICommand ClearTextValue
    {
        get
        {
            clearTextValue ??= new DelegateCommand(OnClearTextValue);
            return clearTextValue;
        }
    }
    private static void OnClearTextValue(object parameter)
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
