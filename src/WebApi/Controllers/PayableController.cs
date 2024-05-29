using System.Text.Json;
using Application.UseCases.Payable.Commands.CreatePayable;
using Application.UseCases.Payable.Commands.CreatePayableBatch;
using Application.UseCases.Payable.Commands.DeletePayable;
using Application.UseCases.Payable.Commands.UpdatePayable;
using Application.UseCases.Payable.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
// [Authorize]
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
    public async Task<DeletePayableResponse> DeletePayable([FromQuery] Guid payableId) =>
        await mediator.Send(new DeletePayableCommand { Id = payableId });

    [HttpPost("Batch", Name = "CreatePayableBatch")]
    public async Task<IActionResult> CreatePayableBatch(IFormFile file)
    {
        if (file.Length <= 0)
        {
            return BadRequest();
        }

        using var streamReader = new StreamReader(file.OpenReadStream());
        var content = await streamReader.ReadToEndAsync();
        var createPayableCommands = JsonSerializer.Deserialize<List<CreatePayableCommand>>(content);

        var command = new CreatePayableBatchCommand();

        if (createPayableCommands == null)
        {
            return BadRequest();
        }

        command.Payables.AddRange(createPayableCommands);

        await mediator.Send(command);

        return Ok();
    }
}
