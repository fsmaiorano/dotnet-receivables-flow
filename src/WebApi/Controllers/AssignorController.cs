using Application.UseCases.Assignor.Commands.CreateAssignor;
using Application.UseCases.Assignor.Commands.DeleteAssignor;
using Application.UseCases.Assignor.Commands.UpdateAssignor;
using Application.UseCases.Assignor.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("/integrations/[controller]")]
public class AssignorController(ILogger<AssignorController> logger, ISender mediator) : ControllerBase
{
    [HttpGet(Name = "GetAssignorById")]
    public async Task<GetAssignorByIdResponse> GetAssignorById([FromQuery] GetAssignorByIdQuery assignorId)
    {
        return await mediator.Send(assignorId);
    }

    [HttpPost(Name = "CreateAssignor")]
    public async Task<CreateAssignorResponse> CreateAssignor([FromBody] CreateAssignorCommand command)
    {
        return await mediator.Send(command);
    }

    [HttpPut(Name = "UpdateAssignor")]
    public async Task<UpdateAssignorResponse> UpdateAssignor([FromQuery] Guid assignorId,
        [FromBody] UpdateAssignorCommand command)
    {
        command.Id = assignorId;
        return await mediator.Send(command);
    }

    [HttpDelete(Name = "DeleteAssignor")]
    public async Task<DeleteAssignorResponse> DeleteAssignor([FromQuery] Guid assignorId)
    {
        return await mediator.Send(new DeleteAssignorCommand { Id = assignorId });
    }
}