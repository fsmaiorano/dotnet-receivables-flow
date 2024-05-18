using Domain.Events.Payable;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Payable.EventHandler;

public class PayableDeletedEventHandler(ILogger<PayableDeletedEventHandler> logger)
    : INotificationHandler<PayableDeletedEvent>
{
    public Task Handle(PayableDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Payable deleted event handled");
        return Task.CompletedTask;
    }
}