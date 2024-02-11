//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Models;
using DocumentFlow.Views.Browsers;

using System.Collections.ObjectModel;

namespace DocumentFlow.ViewModels;

public partial class NavigatorViewModel : ObservableObject, ISelfSingletonLifetime
{
    [ObservableProperty]
    private object? selectedItem;

    private readonly ObservableCollection<NavigatorModel> models = new();

    public NavigatorViewModel()
    {
        models.Add(
            AddFolder("Документы",
                AddFolder("Склад"),
                AddFolder("Производство"),
                AddFolder("Расчёты с контрагентами"),
                AddFolder("Зар. плата")));

        models.Add(AddFolder("Справочники",
            AddItem<MeasurementView>("Единицы измерения"),
            AddItem<OkopfView>("ОКОПФ"),
            AddItem<OkpdtrView>("Профессии / специальности"),
            AddItem<BankView>("Банки"),
            AddItem<PersonView>("Физ. лица"),
            AddItem<ContractorView>("Контрагенты"),
            AddItem<OrganizationView>("Организации"),
            AddFolder("Номенклатура",
                AddItem<WireView>("Типы проводов"),
                AddItem<MaterialView>("Материалы"),
                AddItem<GoodsView>("Продукция")),
            AddFolder("Производственные операции"),
            AddItem<EquipmentView>("Оборудование")));

        NavigatorModel reports = new() { Header = "Отчёты", ImageName = "icons8-folder-16" };
        NavigatorModel system = new() { Header = "Система", ImageName = "icons8-settings-3-16" };
        NavigatorModel about = new() { Header = "О программе", ImageName = "icons8-info-16" };
        NavigatorModel block = new() { Header = "Заблокировать", ImageName = "icons8-exit-16" };

        models.Add(reports);
        models.Add(system);
        models.Add(about);
        models.Add(block);

        Models = new ReadOnlyObservableCollection<NavigatorModel>(models);
    }

    public ReadOnlyObservableCollection<NavigatorModel> Models { get; }

    private static NavigatorModel AddFolder(string header, params NavigatorModel[] items)
    {
        NavigatorModel folder = new() { Header = header, ImageName = "icons8-folder-16" };
        foreach (var item in items)
        {
            folder.Models.Add(item);
        }

        return folder;
    }

    private static NavigatorModel AddItem<V>(string header)
    {
        return new NavigatorModel() { Header = header, ImageName = "icons8-documents-16", ViewType = typeof(V) };
    }

    partial void OnSelectedItemChanged(object? value)
    {
        if (value is NavigatorModel item && item.ViewType != null)
        {
            WeakReferenceMessenger.Default.Send(new EntityListOpenMessage(item.ViewType, item.Header));
        }
    }
}
