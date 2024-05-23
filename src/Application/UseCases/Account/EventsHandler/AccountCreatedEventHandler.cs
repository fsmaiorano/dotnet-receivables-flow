using Domain.Events.Account;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Account.EventsHandler;

public class AccountCreatedEventHandler(ILogger<AccountCreatedEventHandler> logger)
    : INotificationHandler<AccountCreatedEvent>
{
    public Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Account created event handled");
        return Task.CompletedTask;
    }
}