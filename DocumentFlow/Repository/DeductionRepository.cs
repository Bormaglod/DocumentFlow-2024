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

public class DeductionRepository : DirectoryRepository<Deduction>, IDeductionRepository, ITransientLifetime
{
    public DeductionRepository(IDatabase database) : base(database) { }

    protected override QueryParameters GetDefaultSlimQueryParameters()
    {
        return new QueryParameters()
        {
            IncludeDocumentsInfo = false,
            Quantity = QuantityInformation.Full
        };
    }
}