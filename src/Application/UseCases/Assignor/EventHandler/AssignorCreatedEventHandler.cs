using Domain.Events.Assignor;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Assignor.EventHandler;

public class AssignorCreatedEventHandler(ILogger<AssignorCreatedEventHandler> logger)
    : INotificationHandler<AssignorCreatedEvent>
{
    public Task Handle(AssignorCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Assignor created event handled");
        return Task.CompletedTask;
    }
}