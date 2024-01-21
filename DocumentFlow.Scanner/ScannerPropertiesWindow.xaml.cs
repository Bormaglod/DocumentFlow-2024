//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Windows;

using WIA;

namespace DocumentFlow.Scanner;

/// <summary>
/// Логика взаимодействия для ScannerPropertiesWindow.xaml
/// </summary>
public partial class ScannerPropertiesWindow : Window
{
    private readonly Properties properties;

    public ScannerPropertiesWindow(Properties properties)
    {
        InitializeComponent();

        this.properties = properties;
    }

    internal ObservableCollection<ScannerProperty> ScannerProperties = new();

    public static void ShowPropertiesDialog(Properties properties)
    {
        ScannerPropertiesWindow dialog = new(properties);
        dialog.ShowDialog();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        for (int i = 0; i < properties.Count; i++)
        {
            Property property = properties[i + 1];

            var prop = new ScannerProperty()
            {
                Name = property.Name,
                Id = property.PropertyID,
                Value = property.GetValueString()
            };

            var type = (WiaPropertyType)property.Type;
            if (type > WiaPropertyType.UnsupportedPropertyType && type <= WiaPropertyType.CurrencyPropertyType)
            {
                prop.SubType = property.SubType;
                if (property.SubType == WiaSubType.RangeSubType)
                {
                    prop.SubTypeMin = property.SubTypeMin;
                    prop.SubTypeMax = property.SubTypeMax;
                    prop.SubTypeStep = property.SubTypeStep;
                }
                else
                {
                    prop.SubTypeValues = property.SubTypeValues;
                }
            }

            ScannerProperties.Add(prop);
            /*Property property;
            ListViewItem item;
            string value;
            WiaPropertyType type;

            property = properties[i + 1];
            type = (WiaPropertyType)property.Type;

            value = property.GetValueString();
            item = new ListViewItem();

            for (int j = 0; j < listView.Columns.Count; j++)
            {
                item.SubItems.Add(string.Empty);
            }

            item.SubItems[(int)ColumnType.Name].Text = property.Name;
            item.SubItems[(int)ColumnType.Id].Text = property.PropertyID.ToString();
            item.SubItems[(int)ColumnType.Value].Text = value;

            if (type > WiaPropertyType.UnsupportedPropertyType && type <= WiaPropertyType.CurrencyPropertyType)
            {
                if (property.SubType == WiaSubType.RangeSubType)
                {
                    item.SubItems[(int)ColumnType.Min].Text = property.SubTypeMin.ToString();
                    item.SubItems[(int)ColumnType.Max].Text = property.SubTypeMax.ToString();
                    item.SubItems[(int)ColumnType.Step].Text = property.SubTypeStep.ToString();
                }
                else if (property.SubType == WiaSubType.ListSubType)
                {
                    item.SubItems[(int)ColumnType.Values].Text = property.SubTypeValues.ToSeparatedString();
                }
            }

            listView.Items.Add(item);*/
        }
    }
}
