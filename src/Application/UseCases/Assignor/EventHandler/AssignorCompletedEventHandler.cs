using Domain.Events.Assignor;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Assignor.EventHandler;

public class AssignorCompletedEventHandler(ILogger<AssignorCompletedEventHandler> logger)
    : INotificationHandler<AssignorCompletedEvent>
{
    public Task Handle(AssignorCompletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Assignor completed event handled");
        return Task.CompletedTask;
    }
}