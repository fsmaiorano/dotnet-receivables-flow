namespace Domain.Entities;

using Common;

public class PayablesQueueEntity : BaseEntity
{
    public required Guid BatchId { get; set; }
    public required int PayablesBatchQuantity { get; set; }
    public required int PayablesProcessed { get; set; }
    public required int PayablesProcessedSuccess { get; set; }
    public required int PayablesProcessedError { get; set; }
}
