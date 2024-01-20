//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Microsoft.Toolkit.Uwp.Notifications;

namespace DocumentFlow.Common;

public class ToastOperations
{
    static public void OnActivated()
    {
        ToastNotificationManagerCompat.OnActivated += toastArgs =>
        {
            ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

            if (args.Contains("action"))
            {
                if (args["action"] == "folder")
                {
                    WorkOperations.OpenFolder(args["path"]);
                }
                else if (args["action"] == "open")
                {
                    WorkOperations.OpenFile(args["file"]);
                }
            }
        };
    }

    static public void DownloadFileCompleted(string fileName)
    {
        new ToastContentBuilder()
            .AddText("Файл сохранен")
            .AddText(fileName)
                .AddButton(new ToastButton()
                .SetContent("Расположение")
                .AddArgument("action", "folder")
                .AddArgument("path", fileName)
                .SetBackgroundActivation())
            .AddButton(new ToastButton()
                .SetContent("Открыть")
                .AddArgument("action", "open")
                .AddArgument("file", fileName)
                .SetBackgroundActivation())
            .Show(toast =>
            {
                toast.ExpirationTime = DateTime.Now.AddMinutes(1);
            });
    }

    static public void EmailHasBeenSent(string subject, string adresses)
    {
        new ToastContentBuilder()
            .AddArgument("action", "viewConversation")
            .AddArgument("conversationId", 9813)
            .AddText("Документ отправлен")
            .AddText(subject)
            .AddText($"Получатели: {adresses}")
            .Show(toast =>
            {
                toast.ExpirationTime = DateTime.Now.AddMinutes(1);
            });
    }

    static public void IdentifierValueCopied(Guid id)
    {
        new ToastContentBuilder()
            .AddText("Идентификатор записи скопирован буфер:")
            .AddText(id.ToString("B"))
            .Show(toast =>
            {
                toast.ExpirationTime = DateTime.Now.AddSeconds(30);
            });
    }
}
