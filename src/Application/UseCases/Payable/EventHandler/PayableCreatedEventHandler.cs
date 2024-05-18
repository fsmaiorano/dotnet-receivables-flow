using Domain.Events.Payable;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Payable.EventHandler;

public class PayableCreatedEventHandler(ILogger<PayableCreatedEventHandler> logger)
    : INotificationHandler<PayableCreatedEvent>
{
    public Task Handle(PayableCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Payable created event handled");
        return Task.CompletedTask;
    }
}