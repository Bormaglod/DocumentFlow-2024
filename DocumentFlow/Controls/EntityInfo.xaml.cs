//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common;
using DocumentFlow.Models.Entities;

using System.Windows;
using System.Windows.Controls;

namespace DocumentFlow.Controls;

/// <summary>
/// Логика взаимодействия для DocumentInfoView.xaml
/// </summary>
public partial class EntityInfo : UserControl
{
    public EntityInfo()
    {
        InitializeComponent();
    }

    public DocumentInfo DocumentInfo 
    { 
        get { return (DocumentInfo)GetValue(DocumentInfoProperty); }
        set { SetValue(DocumentInfoProperty, value); }
    }

    public static readonly DependencyProperty DocumentInfoProperty = DependencyProperty.Register("DocumentInfo", typeof(DocumentInfo), typeof(EntityInfo));

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(DocumentInfo.Id.ToString() ?? string.Empty);
        ToastOperations.IdentifierValueCopied(DocumentInfo.Id);
    }
}
