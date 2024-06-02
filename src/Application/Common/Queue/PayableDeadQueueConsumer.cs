using Microsoft.Extensions.Configuration;

namespace Application.Common.Queue;

public class PayableDeadQueueConsumer(IConfiguration configuration)
    : BaseQueueConsumer(configuration, configuration["RabbitMqDeadQueueName"]!);
