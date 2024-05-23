using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Account;

public class AccountCompletedEvent(AccountEntity assignor) : BaseEvent
{
    public AccountEntity Account { get; } = assignor;
}