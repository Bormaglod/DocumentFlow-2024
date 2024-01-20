//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace DocumentFlow.Common.Enums;

public enum Privilege 
{ 
    Select, 
    Insert, 
    Update, 
    Delete 
}

public enum SQLCommand
{
    Insert,
    Update,
    Delete,
    Undelete,
    Wipe
}

public enum MessageDestination
{
    Object,
    List
}

public enum MessageAction
{
    Refresh,
    Add,
    Delete
}

public enum WipeAction
{
    Current,
    All,
    Cancel
}

public enum TypeContent 
{ 
    Directory,
    Document
}

public enum SubjectsCivilLow
{
    [Description("Физическое лицо")]
    Person,

    [Description("Юридическое лицо")]
    LegalEntity
}

public enum ContractorType
{
    [Description("С продавцом")]
    Seller,

    [Description("С покупателем")]
    Buyer
}

public enum EmployeeRole
{
    [Description("Не определена")]
    NotDefined,

    [Description("Директор")]
    Director,

    [Description("Гл. бухгалтер")]
    ChiefAccountant,

    [Description("Служащий")]
    Employee,

    [Description("Рабочий")]
    Worker
}

public enum MaterialKind
{
    [Description("Не определён")]
    Undefined,

    [Description("Провод")]
    Wire,

    [Description("Контакт")]
    Terminal,

    [Description("Колодка")]
    Housing,

    [Description("Уплотнитель")]
    Seal
}