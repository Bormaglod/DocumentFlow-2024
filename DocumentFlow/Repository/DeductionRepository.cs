//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.Repository;

public class DeductionRepository(IDatabase database) : 
    DirectoryRepository<Deduction>(database), 
    IDeductionRepository, 
    ITransientLifetime
{
    protected override QueryParameters GetDefaultSlimQueryParameters()
    {
        return new QueryParameters()
        {
            IncludeDocumentsInfo = false,
            Quantity = QuantityInformation.Full
        };
    }
}