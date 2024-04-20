//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IStatesRepository
{
    State Get(short id);
    State Get(IDbConnection connection, short id);
    Task<State> GetAsync(short id);
    Task<State> GetAsync(IDbConnection connection, short id);
    IReadOnlyList<State> GetStateTargets(BaseDocument document);
    IReadOnlyList<State> GetStateTargets(IDbConnection connection, BaseDocument document);
    IReadOnlyList<State> GetStateTargets(Type documentType, State fromState);
    IReadOnlyList<State> GetStateTargets(IDbConnection connection, Type documentType, State fromState);
    Task<IEnumerable<State>> GetStateTargetsAsync(BaseDocument document);
    Task<IEnumerable<State>> GetStateTargetsAsync(IDbConnection connection, BaseDocument document);
    Task<IEnumerable<State>> GetStateTargetsAsync(Type documentType, State fromState);
    Task<IEnumerable<State>> GetStateTargetsAsync(IDbConnection connection, Type documentType, State fromState);
    void SetDocumentState(BaseDocument document, short newState);
    void SetDocumentState(IDbConnection connection, BaseDocument document, short newState, IDbTransaction? transaction);
    Task SetDocumentStateAsync(BaseDocument document, short newState);
    Task SetDocumentStateAsync(IDbConnection connection, BaseDocument document, short newState, IDbTransaction? transaction);
}
