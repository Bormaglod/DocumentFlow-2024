//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Interfaces;

public interface IDependentCollection
{
    List<IDependentEntity> NewItems { get; }
    List<IDependentEntity> UpdateItems { get; }
    List<IDependentEntity> RemoveItems { get; }
    void CompleteChanged();
}
