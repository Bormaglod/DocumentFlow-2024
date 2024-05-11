//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;

namespace DocumentFlow.Common;

public class Length
{
    private const float DefaultDPI = 200;

    private readonly float value;
    private readonly GraphicsUnit unit;
    private readonly float dpi = DefaultDPI;

    private Length(float value, GraphicsUnit unit) => (this.value, this.unit) = (value, unit);
    private Length(float value, float dpi) => (this.value, this.unit, this.dpi) = (value, GraphicsUnit.Dpi, dpi);
    public Length ToInch() => new(Convert(this, GraphicsUnit.Inch), GraphicsUnit.Inch);
    public Length ToPt() => new(Convert(this, GraphicsUnit.Point), GraphicsUnit.Point);
    public Length ToDpi(int dpi) => new(Convert(this, GraphicsUnit.Dpi), dpi);
    public Length ToMillimeter() => new(Convert(this, GraphicsUnit.Millimeter), GraphicsUnit.Millimeter);
    public static Length FromInch(float value) => new(value, GraphicsUnit.Inch);
    public static Length FromMm(float value) => new(value, GraphicsUnit.Millimeter);
    public static Length FromDisplay(float value) => new(value, GraphicsUnit.Display);
    public static Length FromPoint(float value) => new(value, GraphicsUnit.Point);
    public static Length FromDpi(float value, float dpi) => new(value, dpi);

    public static implicit operator float(Length length) => length.value;

    private static float Convert(Length length, GraphicsUnit to, float dpi = DefaultDPI)
    {
        return length.unit switch
        {
            GraphicsUnit.Inch => ConvertFromInch(length.value, to, dpi),
            GraphicsUnit.Centimeter => ConvertFromCentimeter(length.value, to, dpi),
            GraphicsUnit.Display => ConvertFromDisplay(length.value, to, dpi),
            GraphicsUnit.Point => ConvertFromPoint(length.value, to, dpi),
            GraphicsUnit.Millimeter => ConvertFromMillimeter(length.value, to, dpi),
            GraphicsUnit.Dpi => ConvertFromDpi(length.value, length.dpi, to, dpi),
            _ => length.value,
        };
    }

    /// <summary>
    /// Функция преобразует значение metricValue указанное в дюймах в величину определяемую параметром to.
    /// </summary>
    /// <param name="metricValue"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private static float ConvertFromInch(float metricValue, GraphicsUnit to, float dpi)
    {
        float[] metrics = { 1, 2.54f, 25.4f, 96, 72, dpi };
        return metricValue * metrics[(int)to];
    }

    private static float ConvertFromCentimeter(float metricValue, GraphicsUnit to, float dpi) => ConvertFromInch(metricValue, to, dpi) / 2.54f;

    private static float ConvertFromDisplay(float metricValue, GraphicsUnit to, float dpi) => ConvertFromInch(metricValue, to, dpi) / 96;

    private static float ConvertFromPoint(float metricValue, GraphicsUnit to, float dpi) => ConvertFromInch(metricValue, to, dpi) / 72;

    private static float ConvertFromMillimeter(float metricValue, GraphicsUnit to, float dpi) => ConvertFromInch(metricValue, to, dpi) / 25.4f;

    private static float ConvertFromDpi(float metricValue, float fromDPI, GraphicsUnit to, float toDPI) => ConvertFromInch(metricValue, to, toDPI) / fromDPI;
}
