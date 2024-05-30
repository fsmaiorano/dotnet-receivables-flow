namespace Application.Common.Queue;

using UseCases.Payable.Commands.CreatePayable;

public class PayableBatchQueueModel
{
    public required Guid Id { get; set; }
    public required List<CreatePayableCommand> Payables { get; set; }
}
