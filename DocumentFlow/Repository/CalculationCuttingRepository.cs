﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.Repository;

public class CalculationCuttingRepository(IDatabase database) : 
    CalculationItemRepository<CalculationCutting>(database), 
    ICalculationCuttingRepository, 
    ITransientLifetime
{
}
