﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using Syncfusion.Windows.Tools.Controls;

namespace DocumentFlow.Interfaces;

public interface IEntityGridViewModel
{
    DocumentInfo? Owner { get; set; }
    bool AvailableNavigation { get; set; }
    bool AvailableGrouping { get; set; }
    SizeMode SizeMode { get; set; }
    void SetView(IGridPageView view);
}
