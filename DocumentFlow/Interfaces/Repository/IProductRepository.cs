//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IProductRepository<T> : IDirectoryRepository<T>
    where T: Product
{
    decimal GetAveragePrice(T product, DateTime? relevanceDate = null);
    decimal GetAveragePrice(IDbConnection connection, T product, DateTime? relevanceDate = null);
    decimal GetRemainder(T product, DateTime? actualDate = null);
    decimal GetRemainder(IDbConnection connection, T product, DateTime? actualDate = null);
    IReadOnlyList<T> GetProducts(bool withMeasurements = false);
    IReadOnlyList<T> GetProducts(IDbConnection connection, bool withMeasurements = false);
}
