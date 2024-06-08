//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DocumentFlow.Common;
using DocumentFlow.Common.Collections;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Data;
using System.Windows;

namespace DocumentFlow.ViewModels.Editors;

public partial class MaterialViewModel : ProductViewModel<Material>, ISelfTransientLifetime
{
    private readonly IMaterialRepository materialRepository = null!;

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
    private DependentCollection<CompatiblePart>? compatibleParts;

    [ObservableProperty]
    private CompatiblePart? compatibleSelected;

    public MaterialViewModel() { }

    public MaterialViewModel(IMeasurementRepository measurementRepository, IMaterialRepository materialRepository) : base(measurementRepository)
    {
        this.materialRepository = materialRepository;
    }

    #region Commands

    [RelayCommand]
    private void AddCompatiblePart()
    {
        if (CompatibleParts == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new DirectoryItemWindow();
        if (dialog.Get(materialRepository.GetMaterials(conn), null, out var material))
        {
            CompatibleParts.Add(new CompatiblePart() { Compatible = material });
        }
    }

    [RelayCommand]
    private void EditCompatiblePart()
    {
        if (CompatibleParts == null || CompatibleSelected == null || CompatibleSelected.Compatible == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new DirectoryItemWindow();
        if (dialog.Get(materialRepository.GetMaterials(conn), CompatibleSelected.Compatible, out var material))
        {
            CompatibleSelected.Compatible = material;
        }
    }

    [RelayCommand]
    private void DeleteCompatiblePart()
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

    [RelayCommand]
    private void CopyCompatiblePart()
    {
        if (CompatibleParts == null || CompatibleSelected == null || CompatibleSelected.Compatible == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new DirectoryItemWindow();
        if (dialog.Get(materialRepository.GetMaterials(conn), CompatibleSelected.Compatible, out var material))
        {
            CompatibleParts.Add(new CompatiblePart() { Compatible = material });
        }
    }

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

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<Material>(x => x.Measurement)
            .MappingQuery<Material>(x => x.Wire)
            .MappingQuery<Material>(x => x.Cross);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<Measurement, Wire, Material>(connection, (material, measurement, wire, cross) =>
        {
            material.Measurement = measurement;
            material.Wire = wire;
            material.Cross = cross;
            return material;
        });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, Material? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        CrossMaterials = materialRepository.GetCrossMaterials(connection);

        if (entity != null)
        {
            CompatibleParts = new DependentCollection<CompatiblePart>(entity, materialRepository.GetCompatibleParts(connection, entity));
        }
        else
        {
            CompatibleParts ??= [];
        }
    }

    protected override void UpdateDependents(IDbConnection connection, Material material, IDbTransaction? transaction = null)
    {
        if (CompatibleParts != null)
        {
            CompatibleParts.Owner = material;
            connection.UpdateDependents(CompatibleParts, transaction);
        }
    }
}