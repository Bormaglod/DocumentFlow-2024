//-----------------------------------------------------------------------
// Copyright � 2010-2024 �������� ������ ����������. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

namespace DocumentFlow.Interfaces.Repository;

public interface IEmailRepository
{
    Email? Get(string email);
}
