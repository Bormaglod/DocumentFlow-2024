//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

namespace DocumentFlow.Interfaces.Repository;

public interface IStatesRepository
{
    IReadOnlyList<State> GetStateTargets(BaseDocument document);
    IReadOnlyList<State> GetStateTargets(Type documentType, State fromState);
}
