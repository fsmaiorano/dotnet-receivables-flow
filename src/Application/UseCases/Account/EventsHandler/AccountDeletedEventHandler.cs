using Domain.Events.Account;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Account.EventsHandler;

public class AccountDeletedEventHandler(ILogger<AccountDeletedEventHandler> logger)
    : INotificationHandler<AccountDeletedEvent>
{
    public Task Handle(AccountDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Account deleted event handled");
        return Task.CompletedTask;
    }
}