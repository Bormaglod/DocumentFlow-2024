﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Controls;
using DocumentFlow.Interfaces;

namespace DocumentFlow.Views.Browsers;

/// <summary>
/// Логика взаимодействия для OkopfView.xaml
/// </summary>
public partial class OkopfView : BaseViewerControl, ISelfTransientLifetime
{
    public OkopfView()
    {
        InitializeComponent();
    }
}
