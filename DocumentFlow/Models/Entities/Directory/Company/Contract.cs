﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;

using Humanizer;

namespace DocumentFlow.Models.Entities;

public partial class Contract : Directory
{
    [ObservableProperty]
    private bool taxPayer;

    [ObservableProperty]
    private bool isDefault;

    [ObservableProperty]
    private DateTime documentDate = DateTime.Today;

    [ObservableProperty]
    private DateTime dateStart = DateTime.Today;

    [ObservableProperty]
    private DateTime? dateEnd;

    [ObservableProperty]
    [property: ForeignKey(FieldKey = "signatory_id")]
    private Employee? signatory;

    [ObservableProperty]
    [property: ForeignKey(FieldKey = "org_signatory_id")]
    private OurEmployee? orgSignatory;

    [ObservableProperty]
    private short? paymentPeriod;

    public Guid OrganizationId { get; protected set; }

    [EnumType("contractor_type")]
    public string CType { get; set; } = "buyer";

    [DenyWriting]
    public ContractorType ContractorType
    {
        get => Enum.Parse<ContractorType>(CType.Pascalize());
        set => CType = value.ToString().Underscore();
    }

    public override string ToString()
    {
        return $"{ItemName} {Code} от {DocumentDate:d}";
    }
}
