using Domain.Events.Assignor;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Assignor.EventHandler;

public class AssignorDeletedEventHandler(ILogger<AssignorDeletedEventHandler> logger)
    : INotificationHandler<AssignorDeletedEvent>
{
    public Task Handle(AssignorDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Assignor deleted event handled");
        return Task.CompletedTask;
    }
}