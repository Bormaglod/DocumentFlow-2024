//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Common.Collections;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Syncfusion.Windows.Shared;

using System.Data;
using System.Windows;
using System.Windows.Input;

namespace DocumentFlow.ViewModels.Editors;

public partial class MaterialViewModel : ProductViewModel<Material>, ISelfTransientLifetime
{
    [ObservableProperty]
    private decimal? minOrder;

    [ObservableProperty]
    private string? extArticle;

    [ObservableProperty]
    private Wire? wire;

    [ObservableProperty]
    private Material? cross;

    [ObservableProperty]
    private MaterialKind kind;

    [ObservableProperty]
    private IEnumerable<Material>? crossMaterials;

    /// <summary>
    /// Возвращает список совместимых комплектующих.
    /// </summary>
    [ObservableProperty]
    private IList<CompatiblePart>? compatibleParts;

    [ObservableProperty]
    private CompatiblePart? compatibleSelected;

    public MaterialViewModel() { }

    public MaterialViewModel(IDatabase database) : base(database) { }

    #region Commands

    #region AddCompatiblePart

    private ICommand? addCompatiblePart;

    public ICommand AddCompatiblePart
    {
        get
        {
            addCompatiblePart ??= new DelegateCommand(OnAddCompatiblePart);
            return addCompatiblePart;
        }
    }

    private void OnAddCompatiblePart(object parameter)
    {
        if (CompatibleParts == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new DirectoryItemWindow();
        if (dialog.Get(GetForeignDirectory<Material>(conn), null, out var material))
        {
            CompatibleParts.Add(new CompatiblePart() { Compatible = material });
        }
    }

    #endregion

    #region EditCompatiblePart

    private ICommand? editCompatiblePart;

    public ICommand EditCompatiblePart
    {
        get
        {
            editCompatiblePart ??= new DelegateCommand(OnEditCompatiblePart);
            return editCompatiblePart;
        }
    }

    private void OnEditCompatiblePart(object parameter)
    {
        if (CompatibleParts == null || CompatibleSelected == null || CompatibleSelected.Compatible == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new DirectoryItemWindow();
        if (dialog.Get(GetForeignDirectory<Material>(conn), CompatibleSelected.Compatible, out var material))
        {
            CompatibleSelected.Compatible = material;
        }
    }

    #endregion

    #region DeleteCompatiblePart

    private ICommand? deleteCompatiblePart;

    public ICommand DeleteCompatiblePart
    {
        get
        {
            deleteCompatiblePart ??= new DelegateCommand(OnDeleteCompatiblePart);
            return deleteCompatiblePart;
        }
    }

    private void OnDeleteCompatiblePart(object parameter)
    {
        if (CompatibleParts != null && CompatibleSelected != null)
        {
            if (MessageBox.Show("Вы действительно хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            CompatibleParts.Remove(CompatibleSelected);
        }
    }

    #endregion

    #region CopyCompatiblePart

    private ICommand? copyCompatiblePart;

    public ICommand CopyCompatiblePart
    {
        get
        {
            copyCompatiblePart ??= new DelegateCommand(OnCopyCompatiblePart);
            return copyCompatiblePart;
        }
    }

    private void OnCopyCompatiblePart(object parameter)
    {
        if (CompatibleParts == null || CompatibleSelected == null || CompatibleSelected.Compatible == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new DirectoryItemWindow();
        if (dialog.Get(GetForeignDirectory<Material>(conn), CompatibleSelected.Compatible, out var material))
        {
            CompatibleParts.Add(new CompatiblePart() { Compatible = material });
        }
    }

    #endregion

    #endregion

    protected override string GetStandardHeader() => "Материал";

    protected override void RaiseAfterLoadDocument(Material entity)
    {
        base.RaiseAfterLoadDocument(entity);
        MinOrder = entity.MinOrder;
        ExtArticle = entity.ExtArticle;
        Wire = entity.Wire;
        Cross = entity.Cross;
        Kind = entity.Kind;
    }

    protected override void UpdateEntity(Material entity)
    {
        entity.MinOrder = MinOrder;
        entity.ExtArticle = ExtArticle;
        entity.Wire = Wire;
        entity.Cross = Cross;
        entity.Kind = Kind;
    }

    protected override void Load()
    {
        Load<Measurement, Wire, Material>((material, measurement, wire, cross) =>
        {
            material.Measurement = measurement;
            material.Wire = wire;
            material.Cross = cross;
            return material;
        },
        (refs) =>
        {
            if (refs == "material_id")
            {
                return "owner_id";
            }

            return refs;
        });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, Material? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        CrossMaterials = GetForeignDirectory<Material>(connection, callback: q => q.WhereNull("owner_id"));

        if (entity != null)
        {
            var sql = "select cp.*, m.id, m.code, m.item_name from compatible_part as cp left join material m on m.id = cp.compatible_id where cp.owner_id = :Id";
            var res = connection.Query<CompatiblePart, Material, CompatiblePart>(sql, (cp, material) =>
            {
                cp.Compatible = material;
                return cp;
            }, entity);

            CompatibleParts = new DependentCollection<CompatiblePart>(entity, res);
        }
    }

    protected override void UpdateDependents(IDbConnection connection, IDbTransaction? transaction = null)
    {
        if (CompatibleParts != null)
        {
            connection.UpdateDependents(CompatibleParts, transaction);
        }
    }
}