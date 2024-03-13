//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using System.ComponentModel;
using System.Data;

namespace DocumentFlow.ViewModels.Editors;

public partial class CalculationCuttingViewModel : BaseCalculationOperationViewModel<CalculationCutting>, ISelfTransientLifetime
{
    private readonly IMaterialRepository repoMaterial = null!;
    private readonly ICuttingRepository repoCutting = null!;

    public CalculationCuttingViewModel() {}

    public CalculationCuttingViewModel(
        IEquipmentRepository repoEquipment, 
        IMaterialRepository repoMaterial, 
        ICuttingRepository repoCutting) : base(repoEquipment)
    {
        this.repoMaterial = repoMaterial;
        this.repoCutting = repoCutting;
    }

    protected override string GetStandardHeader() => "Резка";

    protected override void InitializeEntityCollections(IDbConnection connection, CalculationCutting? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        Materials = repoMaterial.GetWires(connection);
        Operations = repoCutting.GetOperations(connection);

    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Operation) && Operation is Cutting cutting)
        {
            MaterialAmount = cutting.SegmentLength / 1000m;
        }
    }
}