//-----------------------------------------------------------------------
// Copyright © 2010-2022 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// Date: 27.05.2015
//-----------------------------------------------------------------------

namespace DocumentFlow.Common.Extensions;

public static class LengthExtension
{
    public static Length MmToPt(this float value) => Length.FromMm(value).ToPt();
    public static Length DisplayToMm(this float value) => Length.FromDisplay(value).ToMillimeter();
    public static Length DpiToPoint(this float value, float dpi) => Length.FromDpi(value, dpi).ToPt();
}
