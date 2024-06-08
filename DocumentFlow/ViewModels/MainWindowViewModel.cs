//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common;
using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Models.Settings;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Npgsql;

using Syncfusion.Windows.Tools.Controls;

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DocumentFlow.ViewModels;

public partial class MainWindowViewModel :
    WindowViewModel,
    IRecipient<EntityListOpenMessage>,
    IRecipient<EntityEditorOpenMessage>,
    IRecipient<EditorPageHeaderChangedMessage>,
    IRecipient<RequestClosePageMessage>,
    ISelfSingletonLifetime
{
    private readonly IServiceProvider services;
    private readonly IDatabase database;
    private readonly LocalSettings localSettings;
    private readonly AppSettings appSettings;

    private readonly ConcurrentQueue<NotifyMessage> notifies = new();
    private readonly CancellationTokenSource cancelTokenSource;
    private readonly DispatcherTimer timerDatabaseListen;

    [ObservableProperty]
    private object? activeDocument;

    [ObservableProperty]
    private double navigatorWidth = 250;

    [ObservableProperty]
    private string title = "DocumentFlow";

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    public MainWindowViewModel()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    {
    }

    public MainWindowViewModel(IServiceProvider services, IDatabase database, IOptionsSnapshot<AppSettings> appOptions, IOptionsSnapshot<LocalSettings> localOptions)
    {
        this.services = services;
        this.database = database;
        
        localSettings = localOptions.Value;
        appSettings = appOptions.Value;

        WeakReferenceMessenger.Default.RegisterAll(this);

        RestoreSettings(localSettings.MainWindow.Settings);
        NavigatorWidth = localSettings.MainWindow.NavigatorWidth;

        cancelTokenSource = new CancellationTokenSource();

        timerDatabaseListen = new DispatcherTimer();
        timerDatabaseListen.Tick += new EventHandler(TimerDatabaseListen_Tick);
        timerDatabaseListen.Interval = new TimeSpan(0, 0, 1);
        
        if (appSettings.UseDataNotification)
        {
            timerDatabaseListen.Start();

            CancellationToken token = cancelTokenSource.Token;

            Task.Run(() => CreateListener(token), token);
        }

        Title = $"DocumentFlow {Assembly.GetExecutingAssembly().GetName().Version} - <{database.ConnectionName}>";

        Windows = [];
        Windows.CollectionChanged += Windows_CollectionChanged;
    }

    public ObservableCollection<object> Windows { get; set; }

    #region Commands

    [RelayCommand]
    private void DocumentClosing(CancelingRoutedEventArgs e)
    {
        Windows.Remove(e.OriginalSource);
    }

    [RelayCommand]
    private void AppClosing(CancelEventArgs e)
    {
        SaveSettings(localSettings.MainWindow.Settings);
        localSettings.MainWindow.NavigatorWidth = NavigatorWidth;

        localSettings.Save();

        foreach (var item in Windows.OfType<Control>())
        {
            WeakReferenceMessenger.Default.Send(new PageClosedMessage(item.DataContext));
        }
    }

    #endregion

    #region Receives

    public void Receive(EntityListOpenMessage message)
    {
        if (ActivateOpenedDocument(message.ViewType))
        {
            return;
        }

        var view = services.GetRequiredService(message.ViewType);
        if (view is FrameworkElement element && element is IGridPageView pageView)
        {
            DocumentContainer.SetHeader(element, message.Text);

            Windows.Add(pageView);
        }
    }

    public void Receive(EntityEditorOpenMessage message)
    {
        if (message.DocumentId != null)
        {
            if (ActivateOpenedDocument(message.EditorType, message.DocumentId))
            {
                return;
            }
        }

        var view = services.GetRequiredService(message.EditorType);
        if (view is FrameworkElement element && element is IEditorPageView pageView)
        {
            if (element.DataContext is IEntityEditorViewModel model)
            {
                if (message.DocumentId.HasValue)
                {
                    try
                    {
                        model.LoadDocument(message.DocumentId.Value, message.Options);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else 
                {
                    model.CreateDocument(message.Options);
                }
                
                model.View = pageView;

                DocumentContainer.SetHeader(element, model.Header);
            }

            Windows.Add(pageView);
        }
    }

    public void Receive(EditorPageHeaderChangedMessage message)
    {
        var page = Windows.FirstOrDefault(x => x == message.Page);
        if (page is DependencyObject dependency) 
        {
            DocumentContainer.SetHeader(dependency, message.Header);
        }
    }

    public void Receive(RequestClosePageMessage message)
    {
        Windows.Remove(message.Value);
    }

    #endregion

    private async Task CreateListener(CancellationToken token)
    {
        await using var conn = new NpgsqlConnection(database.ConnectionString);
        await conn.OpenAsync(token);

        conn.Notification += (o, e) =>
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                    {
                        new JsonStringEnumConverter()
                    }
            };
            NotifyMessage? message = JsonSerializer.Deserialize<NotifyMessage>(e.Payload, options);
            if (message != null)
            {
                notifies.Enqueue(message);
            }
        };

        await using (var cmd = new NpgsqlCommand("LISTEN db_notification", conn))
        {
            cmd.ExecuteNonQuery();
        }

        while (!token.IsCancellationRequested)
        {
            await conn.WaitAsync(token);
        }

        await Task.CompletedTask;
    }

    private bool ActivateOpenedDocument(Type type, Guid? id = null)
    {
        Control? openedView;
        if (id == null)
        {
            openedView = Windows.OfType<Control>().FirstOrDefault(x => x.GetType().Name == type.Name);
        }
        else
        {
            openedView = Windows.OfType<Control>().FirstOrDefault(x => x.DataContext is IEntityEditorViewModel model && model.Id == id);
        }

        if (openedView != null)
        {
            var container = DocumentContainer.GetDocumentContainer(openedView);
            container.SelectItem(openedView);
            return true;
        }

        return false;
    }

    private void TimerDatabaseListen_Tick(object? sender, EventArgs e) 
    {
        if (appSettings.UseDataNotification && notifies.TryDequeue(out NotifyMessage? notify))
        {
            if (string.IsNullOrEmpty(notify.EntityName))
            {
                return;
            }

            EntityActionMessage? message = null;
            switch (notify.Destination)
            {
                case MessageDestination.Object:
                    message = new(notify.EntityName, notify.ObjectId, notify.Action);
                    break;
                case MessageDestination.List:
                    if (notify.ObjectId == Guid.Empty)
                    {
                        message = new(notify.EntityName);
                    }
                    else
                    {
                        message = new(notify.EntityName, notify.ObjectId);
                    }

                    break;
                default:
                    break;
            }

            if (message != null)
            {
                message.Source = MessageActionSource.Outer;
                WeakReferenceMessenger.Default.Send(message);
            }
        }
    }

    private void Windows_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems.OfType<Control>())
                    {
                        WeakReferenceMessenger.Default.Send(new PageClosedMessage(item.DataContext));
                    }
                }

                break;
        }
    }
}
