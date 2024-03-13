//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IEquipmentRepository : IDirectoryRepository<Equipment>
{
    IReadOnlyList<Equipment> GetEquipments();
    IReadOnlyList<Equipment> GetEquipments(IDbConnection connection);
    IReadOnlyList<Equipment> GetTools();
    IReadOnlyList<Equipment> GetTools(IDbConnection connection);
}
