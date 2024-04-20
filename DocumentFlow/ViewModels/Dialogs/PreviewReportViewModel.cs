//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Messages;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;
using DocumentFlow.Models.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;

using MimeKit;
using Microsoft.Extensions.Options;

using Syncfusion.Windows.Shared;

using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Pdf = Syncfusion.Windows.PdfViewer;

namespace DocumentFlow.ViewModels.Dialogs;

public partial class PreviewReportViewModel : WindowViewModel, IRecipient<OpenPdfReportMessage>, ISelfTransientLifetime
{
    private readonly LocalSettings? settings;
    private Pdf.PdfViewerControl? pdfViewer;
    private string? pdfFile;
    private DocumentInfo? document;

    [ObservableProperty]
    private int zoomItem;

    public PreviewReportViewModel() { }

    public PreviewReportViewModel(IOptionsSnapshot<LocalSettings> options) : this()
    {
        settings = options.Value;

        RestoreSettings(settings.PreviewPdf.Settings);

        WeakReferenceMessenger.Default.Register(this);
    }

    #region Commands

    #region FitPageCommand

    private ICommand? fitPageCommand;

    public ICommand FitPageCommand
    {
        get
        {
            fitPageCommand ??= new DelegateCommand(OnFitPageCommand);
            return fitPageCommand;
        }
    }

    private void OnFitPageCommand(object parameter)
    {
        if (pdfViewer != null)
        {
            pdfViewer.ZoomMode = Pdf.ZoomMode.FitPage;
        }
    }

    #endregion

    #region FitWidthCommand

    private ICommand? fitWidthCommand;

    public ICommand FitWidthCommand
    {
        get
        {
            fitWidthCommand ??= new DelegateCommand(OnFitWidthCommand);
            return fitWidthCommand;
        }
    }

    private void OnFitWidthCommand(object parameter)
    {
        if (pdfViewer != null)
        {
            pdfViewer.ZoomMode = Pdf.ZoomMode.FitWidth;
        }
    }

    #endregion

    #region PrintCommand

    private ICommand? printCommand;

    public ICommand PrintCommand
    {
        get
        {
            printCommand ??= new DelegateCommand(OnPrintCommand);
            return printCommand;
        }
    }

    private void OnPrintCommand(object parameter)
    {
        pdfViewer?.Print(true);
    }

    #endregion

    #region SaveCommand

    private ICommand? saveCommand;

    public ICommand SaveCommand
    {
        get
        {
            saveCommand ??= new DelegateCommand(OnSaveCommand);
            return saveCommand;
        }
    }

    private void OnSaveCommand(object parameter)
    {
        if (pdfViewer == null)
        {
            return;
        }

        Microsoft.Win32.SaveFileDialog saveFile = new()
        {
            Filter = "Pdf Files|*.pdf"
        };

        if (saveFile.ShowDialog() == true)
        {
            pdfViewer.Save(saveFile.FileName);
        }
    }

    #endregion

    #region SendCommand

    private ICommand? sendCommand;

    public ICommand SendCommand
    {
        get
        {
            sendCommand ??= new DelegateCommand(OnSendCommand);
            return sendCommand;
        }
    }

    private void OnSendCommand(object parameter)
    {
        if (File.Exists(pdfFile) && document != null)
        {
            EmailSendWindow sendWindow = new();
            sendWindow.Attachments.Add(pdfFile);
            if (sendWindow.ShowDialog(out var info) == true)
            {
                var email = ServiceLocator.Context.GetService<IEmailRepository>().Get(info.From.Email);
                if (email == null)
                {
                    MessageBox.Show($"Для адреса <{info.From.Email}> не указаны параметры SMTP-сревера", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _ = SendEmailAsync(document, email, info);
            }
        }
    }

    #endregion

    #region WindowClosing

    private ICommand? windowClosing;

    public ICommand WindowClosing
    {
        get
        {
            windowClosing ??= new DelegateCommand<CancelEventArgs>(OnWindowClosing);
            return windowClosing;
        }
    }

    public void OnWindowClosing(CancelEventArgs e)
    {
        if (settings != null)
        {
            SaveSettings(settings.PreviewPdf.Settings);

            settings.Save();
        }
    }

    #endregion

    #endregion

    public void Receive(OpenPdfReportMessage message)
    {
        pdfFile = message.PdfFile;
        document = message.Document;

        pdfViewer?.Load(pdfFile);
    }

    public void SetViewer(Pdf.PdfViewerControl pdfViewer) => this.pdfViewer = pdfViewer;

    partial void OnZoomItemChanged(int value)
    {
        pdfViewer?.ZoomTo(value);
    }

    private static async Task SendEmailAsync(DocumentInfo document, Email email, EmailInfo info)
    {
        using var client = new SmtpClient();
        var log = info.To.Select(e => new EmailLog()
        {
            EmailId = email.Id,
            ToAddress = e.Email,
            DocumentId = document.Id,
            ContractorId = e.Company?.Id ?? throw new Exception("Не определен контрагент - получатель письма.")
        });

        // SslHandshakeException: An error occurred while attempting to establish an SSL or TLS connection
        // https://stackoverflow.com/questions/59026301/sslhandshakeexception-an-error-occurred-while-attempting-to-establish-an-ssl-or
        //
        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

        await client.ConnectAsync(email.MailHost, email.MailPort, SecureSocketOptions.Auto);

        try
        {
            await client.AuthenticateAsync(email.Address, email.UserPassword);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            return;
        }

        MimeMessage message = new();
        message.From.Add(new MailboxAddress(info.From.Name, info.From.Email));
        message.Bcc.Add(new MailboxAddress(info.From.Name, info.From.Email));
        foreach (EmailAddress e in info.To)
        {
            message.To.Add(new MailboxAddress(e.Name, e.Email));
        }

        message.Subject = info.Subject ?? "Без темы";

        BodyBuilder builder = new();
        if (!string.IsNullOrEmpty(email.SignaturePlain))
        {
            if (string.IsNullOrEmpty(info.Body))
            {
                builder.TextBody = string.Format("\n--\n{0}", email.SignaturePlain);
            }
            else
            {
                builder.TextBody = string.Format("{0}\n--\n{1}", info.Body, email.SignaturePlain);
            }
        }

        if (!string.IsNullOrEmpty(email.SignatureHtml))
        {
            if (string.IsNullOrEmpty(info.Body))
            {
                builder.HtmlBody = string.Format("<br/>{0}", email.SignatureHtml);
            }
            else
            {
                builder.HtmlBody = string.Format("<p>{0}</p><br/>{1}", info.Body, email.SignatureHtml);
            }
        }

        foreach (string attachment in info.Attachments)
        {
            builder.Attachments.Add(attachment);
        }

        message.Body = builder.ToMessageBody();

        await client.SendAsync(message);
        client.Disconnect(true);

        await ServiceLocator.Context.GetService<IEmailLogRepository>().LogAsync(log);

        if (document is BaseDocument baseDocument)
        {
            var states = ServiceLocator.Context.GetService<IStatesRepository>();
            var res = await states.GetStateTargetsAsync(baseDocument);
            if (res.FirstOrDefault(s => s.Id == 3001) != null)
            {
                await states.SetDocumentStateAsync(baseDocument, 3001);
                baseDocument.State = await states.GetAsync(3001);
            }
        }

        ToastOperations.EmailHasBeenSent(
            message.Subject,
            string.Join(", ", info.To.Select(x => x.Name)));
    }
}
