//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для EmailSendWindow.xaml
/// </summary>
[INotifyPropertyChanged]
public partial class EmailSendWindow : Window
{
    [ObservableProperty]
    private ObservableCollection<EmailAddress> emailFrom = new();

    [ObservableProperty]
    private ObservableCollection<EmailAddress> emailTo = new();

    [ObservableProperty]
    private ObservableCollection<EmailAddress> emailToSelected = new();

    [ObservableProperty]
    private EmailAddress? emailFromSelected;
    
    [ObservableProperty]
    private ObservableCollection<string> attachments = new();

    [ObservableProperty]
    private int selectedIndex = -1;

    [ObservableProperty]
    private string? subject;

    [ObservableProperty]
    private string? body;

    public EmailSendWindow()
    {
        InitializeComponent();

        var orgs = ServiceLocator.Context.GetService<IOrganizationRepository>().GetEmails();
        var ourEmps = ServiceLocator.Context.GetService<IOurEmployeeRepository>().GetEmails();

        EmailFrom = new(orgs.Concat(ourEmps));
        EmailTo = new(ServiceLocator.Context.GetService<IEmployeeRepository>().GetEmails());
    }

    public bool ShowDialog([MaybeNullWhen(false)] out EmailInfo emailInfo)
    {
        if (ShowDialog() == true)
        {
            emailInfo = new()
            {
                From = EmailFromSelected!,
                To = EmailToSelected,
                Attachments = Attachments,
                Subject = Subject,
                Body = Body
            };

            return true;
        }

        emailInfo = default;
        return false;
    }

    private void AcceptClick(object sender, RoutedEventArgs e)
    {
        if (EmailFromSelected == null)
        {
            MessageBox.Show("Не указан адрес отправителя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (EmailToSelected == null || EmailToSelected.Count == 0)
        {
            MessageBox.Show("Не указан адрес получателя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        DialogResult = true;
    }

    private void ButtonAdd_Click(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFileDialog fileDialog = new()
        {
            Multiselect = true
        };

        if (fileDialog.ShowDialog() == true)
        {
            foreach (string file in fileDialog.FileNames)
            {
                Attachments.Add(file);
            }
        }
    }

    private void ButtonDelete_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedIndex != -1)
        {
            Attachments.RemoveAt(SelectedIndex);
        }
    }
}
