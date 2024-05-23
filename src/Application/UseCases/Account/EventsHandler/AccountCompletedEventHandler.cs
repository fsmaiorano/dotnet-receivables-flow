using Domain.Events.Account;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Account.EventsHandler;

public class AccountCompletedEventHandler(ILogger<AccountCompletedEventHandler> logger)
    : INotificationHandler<AccountCompletedEvent>
{
    public Task Handle(AccountCompletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Account completed event handled");
        return Task.CompletedTask;
    }
}