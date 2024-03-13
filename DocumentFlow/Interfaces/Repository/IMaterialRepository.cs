//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IMaterialRepository : IProductRepository<Material>
{
    /// <summary>
    /// Функция возвращает список материалов без проводов.
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<Material> GetMaterials();

    /// <summary>
    /// Функция возвращает список материалов без проводов.
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    IReadOnlyList<Material> GetMaterials(IDbConnection connection);

    /// <summary>
    /// Функция возвращает список проводов.
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<Material> GetWires();

    /// <summary>
    /// Функция возвращает список проводов.
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    IReadOnlyList<Material> GetWires(IDbConnection connection);

    /// <summary>
    /// Функция возвращает список материалов, которые могут выступать в качестве 
    /// оригинальных комплектующих.
    /// </summary>
    /// <returns>Список материалов, которые могут выступать в качестве оригинальных комплектующих.</returns>
    IReadOnlyList<Material> GetCrossMaterials();

    /// <summary>
    /// Функция возвращает список материалов, которые могут выступать в качестве 
    /// оригинальных комплектующих.
    /// </summary>
    /// <param name="connection"></param>
    /// <returns>Список материалов, которые могут выступать в качестве оригинальных комплектующих.</returns>
    IReadOnlyList<Material> GetCrossMaterials(IDbConnection connection);

    /// <summary>
    /// Функция возвращает список материалов, которые могут выступать в качестве 
    /// оригинальных комплектующих (за исключением материала, указанного 
    /// в параметре exceptMaterial).
    /// </summary>
    /// <param name="exceptMaterial">Идентификатор материала, который будет исключён из 
    /// списка материалов возвращаемого функцией.</param>
    /// <returns>Список материалов, которые могут выступать в качестве 
    /// оригинальных комплектующих.</returns>
    IReadOnlyList<Material> GetCrossMaterials(Guid exceptMaterial);

    /// <summary>
    /// Функция возвращает список материалов, которые могут выступать в качестве 
    /// оригинальных комплектующих (за исключением материала, указанного 
    /// в параметре exceptMaterial).
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="exceptMaterial">Идентификатор материала, который будет исключён из 
    /// списка материалов возвращаемого функцией.</param>
    /// <returns>Список материалов, которые могут выступать в качестве 
    /// оригинальных комплектующих.</returns>
    IReadOnlyList<Material> GetCrossMaterials(IDbConnection connection, Guid exceptMaterial);

    /// <summary>
    /// Функция возвращает список совместимых материалов с указанным.
    /// </summary>
    /// <param name="material"></param>
    /// <returns></returns>
    IList<CompatiblePart> GetCompatibleParts(Material material);

    /// <summary>
    /// Функция возвращает список совместимых материалов с указанным.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="material"></param>
    /// <returns></returns>
    IList<CompatiblePart> GetCompatibleParts(IDbConnection connection, Material material);
}
