using Application.UseCases.Payable.Commands.CreatePayable;
using Application.UseCases.Payable.Commands.DeletePayable;
using Application.UseCases.Payable.Commands.UpdatePayable;
using Application.UseCases.Payable.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("/integrations/[controller]")]
public class PayableController(ILogger<PayableController> logger, ISender mediator) : ControllerBase
{
    [HttpGet(Name = "GetPayableById")]
    public async Task<GetPayableByIdResponse> GetPayableById([FromQuery] GetPayableByIdQuery payableId)
    {
        return await mediator.Send(payableId);
    }

    [HttpPost(Name = "CreatePayable")]
    public async Task<CreatePayableResponse> CreatePayable([FromQuery] Guid assignorId,
        [FromBody] CreatePayableCommand command)
    {
        command.AssignorId = assignorId;
        return await mediator.Send(command);
    }

    [HttpPut(Name = "UpdatePayable")]
    public async Task<UpdatePayableResponse> UpdatePayable([FromQuery] Guid payableId,
        [FromBody] UpdatePayableCommand command)
    {
        command.Id = payableId;
        return await mediator.Send(command);
    }

    [HttpDelete(Name = "DeletePayable")]
    public async Task<DeletePayableResponse> DeletePayable([FromQuery] Guid payableId)
    {
        return await mediator.Send(new DeletePayableCommand { Id = payableId });
    }
}