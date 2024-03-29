//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;

using System.Windows;

namespace DocumentFlow;

public partial class MainWindow : Window, ISelfSingletonLifetime
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
