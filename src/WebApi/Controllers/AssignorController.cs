using Application.UseCases.Assignor.Commands.CreateAssignor;
using Application.UseCases.Assignor.Commands.DeleteAssignor;
using Application.UseCases.Assignor.Commands.UpdateAssignor;
using Application.UseCases.Assignor.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("/integrations/[controller]")]
public class AssignorController(ILogger<AssignorController> logger, ISender mediator) : ControllerBase
{
    [HttpGet(Name = "GetAssignorById"), Authorize]
    public async Task<GetAssignorByIdResponse> GetAssignorById([FromQuery] GetAssignorByIdQuery assignorId)
    {
        return await mediator.Send(assignorId);
    }

    [HttpPost(Name = "CreateAssignor"), Authorize]
    public async Task<CreateAssignorResponse> CreateAssignor([FromBody] CreateAssignorCommand command)
    {
        return await mediator.Send(command);
    }

    [HttpPut(Name = "UpdateAssignor"), Authorize]
    public async Task<UpdateAssignorResponse> UpdateAssignor([FromQuery] Guid assignorId,
        [FromBody] UpdateAssignorCommand command)
    {
        command.Id = assignorId;
        return await mediator.Send(command);
    }

    [HttpDelete(Name = "DeleteAssignor"), Authorize]
    public async Task<DeleteAssignorResponse> DeleteAssignor([FromQuery] Guid assignorId)
    {
        return await mediator.Send(new DeleteAssignorCommand { Id = assignorId });
    }
}