//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata.Execution;

namespace DocumentFlow.Repository;

public class EmailRepository(IDatabase database) : IEmailRepository, ITransientLifetime
{
    private readonly IDatabase database = database;

    public Email? Get(string email)
    {
        using var conn = database.OpenConnection();

        QueryParameters parameters = new()
        {
            Quantity = QuantityInformation.Full
        };

        return conn.GetQuery<Email>(parameters)
            .Where("address", email)
            .FirstOrDefault<Email>();
    }
}
