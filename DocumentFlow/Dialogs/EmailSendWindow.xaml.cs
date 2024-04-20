//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

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
public partial class EmailSendWindow : Window, INotifyPropertyChanged
{
    private ObservableCollection<EmailAddress> emailFrom = new();
    private ObservableCollection<EmailAddress> emailTo = new();
    private ObservableCollection<string> attachments = new();
    private int selectedIndex = -1;
    private string? subject;
    private string? body;
    private EmailAddress? emailFromSelected;
    private ObservableCollection<EmailAddress> emailToSelected = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public EmailSendWindow()
    {
        InitializeComponent();

        var orgs = ServiceLocator.Context.GetService<IOrganizationRepository>().GetEmails();
        var ourEmps = ServiceLocator.Context.GetService<IOurEmployeeRepository>().GetEmails();

        EmailFrom = new(orgs.Concat(ourEmps));
        EmailTo = new(ServiceLocator.Context.GetService<IEmployeeRepository>().GetEmails());
    }

    public ObservableCollection<EmailAddress> EmailFrom
    {
        get => emailFrom;
        set
        {
            if (emailFrom != value)
            {
                emailFrom = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EmailFrom)));
            }
        }
    }

    public ObservableCollection<EmailAddress> EmailTo
    {
        get => emailTo;
        set
        {
            if (emailTo != value)
            {
                emailTo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EmailTo)));
            }
        }
    }

    public ObservableCollection<EmailAddress> EmailToSelected
    {
        get => emailToSelected;
        set
        {
            if (emailToSelected != value)
            {
                emailToSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EmailToSelected)));
            }
        }
    }

    public EmailAddress? EmailFromSelected
    {
        get => emailFromSelected;
        set
        {
            if (emailFromSelected != value)
            {
                emailFromSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EmailFromSelected)));
            }
        }
    }

    public ObservableCollection<string> Attachments
    {
        get => attachments;
        set
        {
            if (attachments != value)
            {
                attachments = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Attachments)));
            }
        }
    }

    public int SelectedIndex
    {
        get => selectedIndex;
        set
        {
            if (selectedIndex != value)
            {
                selectedIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
            }
        }
    }

    public string? Subject
    {
        get => subject;
        set
        {
            if (subject != value)
            {
                subject = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Subject)));
            }
        }
    }

    public string? Body
    {
        get => body;
        set
        {
            if (body != value)
            {
                body = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Body)));
            }
        }
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
