using Domain.Events.Payable;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Payable.EventHandler;

public class PayableCompletedEventHandler(ILogger<PayableCompletedEventHandler> logger)
    : INotificationHandler<PayableCompletedEvent>
{
    public Task Handle(PayableCompletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Payable completed event handled");
        return Task.CompletedTask;
    }
}