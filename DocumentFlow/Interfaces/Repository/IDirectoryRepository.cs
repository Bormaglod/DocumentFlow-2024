//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IDirectoryRepository<T> : IReadOnlyRepository<T>
    where T : Directory
{
    IReadOnlyList<T> GetSlim(DocumentInfo owner);
    IReadOnlyList<T> GetSlim(IDbConnection connection, DocumentInfo owner);
}
