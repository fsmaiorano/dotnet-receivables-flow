namespace Application.Common.Queue;

using Microsoft.Extensions.Configuration;

public class PayableRetryQueueConsumer3(IConfiguration configuration)
    : BaseQueueConsumer(configuration, configuration["RabbitMqRetryQueueName3"]!);
