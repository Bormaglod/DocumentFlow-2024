//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using System.ComponentModel;
using System.Data;

namespace DocumentFlow.ViewModels.Editors;

public partial class CuttingViewModel : BaseOperationViewModel<Cutting>, ISelfTransientLifetime, IDataErrorInfo
{
    private readonly ICuttingRepository repoCutting = null!;

    [ObservableProperty]
    private int segmentLength;

    [ObservableProperty]
    private decimal leftCleaning;

    [ObservableProperty]
    private int leftSweep;

    [ObservableProperty]
    private decimal rightCleaning;

    [ObservableProperty]
    private int rightSweep;

    [ObservableProperty]
    private int? programNumber;

    [ObservableProperty]
    private IEnumerable<int>? programs;

    public CuttingViewModel() { }

    public CuttingViewModel(ICuttingRepository repoCutting) : base()
    {
        this.repoCutting = repoCutting;
    }

    #region Commands

    [RelayCommand]
    private void ClearProgramNumber()
    {
        ProgramNumber = null;
    }

    #endregion

    public string Error => string.Empty;

    public string this[string name]
    {
        get
        {
            string result = string.Empty;

            switch (name)
            {
                case "SegmentLength":
                    if (SegmentLength < 10 || SegmentLength > 99999)
                    {
                        result = "Длина провода должна быть в пределах 10 мм - 100 м.";
                    }

                    break;
                case "LeftCleaning":
                    if (LeftCleaning < 1 || LeftCleaning > 99)
                    {
                        result = "Длина зачистки должна быть в пределах 1 мм - 100 мм.";
                    }

                    break;
                case "RightCleaning":
                    if (RightCleaning < 1 || RightCleaning > 99)
                    {
                        result = "Длина зачистки должна быть в пределах 1 мм - 100 мм.";
                    }

                    break;
                case "LeftSweep":
                    if (LeftSweep < 0 || LeftSweep > 9)
                    {
                        result = "Ширина окна должна быть в пределах 10 мм.";
                    }

                    break;
                case "RightSweep":
                    if (RightSweep < 0 || RightSweep > 9)
                    {
                        result = "Ширина окна должна быть в пределах 10 мм.";
                    }

                    break;
            }

            return result;
        }
    }

    protected override string GetStandardHeader() => "Резка";

    protected override void RaiseAfterLoadDocument(Cutting entity)
    {
        base.RaiseAfterLoadDocument(entity);
        SegmentLength = entity.SegmentLength;
        LeftCleaning = entity.LeftCleaning;
        LeftSweep = entity.LeftSweep;
        RightCleaning = entity.RightCleaning;
        RightSweep = entity.RightSweep;
        ProgramNumber = entity.ProgramNumber;
    }

    protected override void UpdateEntity(Cutting entity)
    {
        base.UpdateEntity(entity);
        entity.SegmentLength = SegmentLength;
        entity.LeftCleaning = LeftCleaning;
        entity.LeftSweep = LeftSweep;
        entity.RightCleaning = RightCleaning;
        entity.RightSweep = RightSweep;
        entity.ProgramNumber = ProgramNumber;
    }

    protected override void InitializeEntityCollections(IDbConnection connection, Cutting? cutting = null)
    {
        base.InitializeEntityCollections(connection, cutting);

        Programs = repoCutting.GetAvailableProgram(connection, cutting);
    }
}
