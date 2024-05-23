using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Account;

public class AccountDeletedEvent(AccountEntity assignor) : BaseEvent
{
    public AccountEntity Account { get; } = assignor;
}