//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.Windows;
using System.Windows.Controls;

namespace DocumentFlow.Controls;

/// <summary>
/// Логика взаимодействия для DocumentsListView.xaml
/// </summary>
public partial class DocumentsList : UserControl
{
    public DocumentsList()
    {
        InitializeComponent();
    }

    public DocumentInfo DocumentInfo
    {
        get { return (DocumentInfo)GetValue(DocumentInfoProperty); }
        set { SetValue(DocumentInfoProperty, value); }
    }

    public static readonly DependencyProperty DocumentInfoProperty = DependencyProperty.Register(
        "DocumentInfo",
        typeof(DocumentInfo),
        typeof(DocumentsList),
        new FrameworkPropertyMetadata(OnDocumentInfoChanged));

    private static void OnDocumentInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        ((DocumentsList)d).gridContent.ItemsSource = conn.Query<DocumentRefs>("select * from document_refs where owner_id = :Id", e.NewValue); ;
    }
}
