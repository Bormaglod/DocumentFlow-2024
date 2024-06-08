//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

using Microsoft.Extensions.Options;

using MimeKit;

using System.ComponentModel;
using System.IO;
using System.Windows;

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

    [RelayCommand]
    private void FitPage()
    {
        if (pdfViewer != null)
        {
            pdfViewer.ZoomMode = Pdf.ZoomMode.FitPage;
        }
    }

    [RelayCommand]
    private void FitWidth()
    {
        if (pdfViewer != null)
        {
            pdfViewer.ZoomMode = Pdf.ZoomMode.FitWidth;
        }
    }

    [RelayCommand]
    private void Print() => pdfViewer?.Print(true);

    [RelayCommand]
    private void Save()
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

    [RelayCommand]
    private void Send()
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

    [RelayCommand]
    public void WindowClosing(CancelEventArgs e)
    {
        if (settings != null)
        {
            SaveSettings(settings.PreviewPdf.Settings);

            settings.Save();
        }
    }

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
