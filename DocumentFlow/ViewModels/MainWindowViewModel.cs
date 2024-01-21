//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common;
using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Models.Entities;
using DocumentFlow.Models.Settings;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Minio;

using Npgsql;

using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DocumentFlow.ViewModels;

public partial class MainWindowViewModel :
    ObservableObject,
    IRecipient<EntityListOpenMessage>,
    IRecipient<EntityEditorOpenMessage>,
    IRecipient<EditorPageHeaderChangedMessage>,
    IRecipient<ClosePageMessage>,
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
    private WindowState windowState;

    [ObservableProperty]
    private object? activeDocument;

    [ObservableProperty]
    private double left;

    [ObservableProperty]
    private double top;

    [ObservableProperty]
    private double width = 800;

    [ObservableProperty]
    private double height = 450;

    [ObservableProperty]
    private double navigatorWidth = 250;

    [ObservableProperty]
    private string title = "DocumentFlow";

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    public MainWindowViewModel()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    {
    }

    public MainWindowViewModel(IServiceProvider services, IDatabase database, IOptions<AppSettings> appOptions, IOptionsSnapshot<LocalSettings> localOptions)
    {
        this.services = services;
        this.database = database;
        
        localSettings = localOptions.Value;
        appSettings = appOptions.Value;

        WeakReferenceMessenger.Default.RegisterAll(this);

        if (this.localSettings.MainWindow.WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }
        else
        {
            WindowState = this.localSettings.MainWindow.WindowState;
        }

        if (WindowState == WindowState.Normal)
        {
            Left = this.localSettings.MainWindow.Left;
            Top = this.localSettings.MainWindow.Top; ;

            Width = this.localSettings.MainWindow.Width;
            Height = this.localSettings.MainWindow.Height;
        }

        NavigatorWidth = this.localSettings.MainWindow.NavigatorWidth;

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
    }

    public ObservableCollection<object> Windows { get; set; } = new ObservableCollection<object>();

    #region Commands

    #region DocumentClosing

    private ICommand? documentClosing;

    public ICommand DocumentClosing
    {
        get
        {
            documentClosing ??= new DelegateCommand<CancelingRoutedEventArgs>(OnDocumentClosing);
            return documentClosing;
        }
    }

    private void OnDocumentClosing(CancelingRoutedEventArgs e)
    {
        Windows.Remove(e.OriginalSource);
    }

    #endregion

    #endregion

    public void Receive(EntityListOpenMessage message)
    {
        if (ActivateOpenedDocument(message.ViewType))
        {
            return;
        }

        var view = services.GetRequiredService(message.ViewType);
        if (view is FrameworkElement element && element is IGridPageView pageView)
        {
            if (element.DataContext is IEntityGridViewModel model)
            {
                model.View = pageView;
            }

            DocumentContainer.SetHeader(element, message.Text);

            Windows.Add(pageView);
        }
    }

    public void Receive(EntityEditorOpenMessage message)
    {
        if (message.Document != null)
        {
            if (ActivateOpenedDocument(message.EditorType, message.Document.Id))
            {
                return;
            }
        }

        var view = services.GetRequiredService(message.EditorType);
        if (view is FrameworkElement element && element is IEditorPageView pageView)
        {
            if (element.DataContext is IEntityEditorViewModel model)
            {
                if (message.Document != null)
                {
                    try
                    {
                        model.LoadDocument(message.Document, message.Options);
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

    public void Receive(ClosePageMessage message)
    {
        Windows.Remove(message.Value);
    }

    public void OnWindowClosing(object? sender, CancelEventArgs e)
    {
        localSettings.MainWindow.WindowState = WindowState;
        localSettings.MainWindow.Left = Left;
        localSettings.MainWindow.Top = Top;
        localSettings.MainWindow.Width = Width;
        localSettings.MainWindow.Height = Height;
        localSettings.MainWindow.NavigatorWidth = NavigatorWidth;

        localSettings.Save();
    }

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
                WeakReferenceMessenger.Default.Send(message);
            }
        }
    }
}
