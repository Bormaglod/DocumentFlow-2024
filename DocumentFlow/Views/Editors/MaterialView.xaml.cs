﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;

using System.Windows.Controls;

namespace DocumentFlow.Views.Editors;

/// <summary>
/// Логика взаимодействия для MaterialView.xaml
/// </summary>
public partial class MaterialView : UserControl, IEditorPageView, ISelfTransientLifetime
{
    public MaterialView()
    {
        InitializeComponent();
    }
}
