namespace Application.Common.Queue;

using Microsoft.Extensions.Configuration;

public class PayableRetryQueueConsumer2(IConfiguration configuration)
    : BaseQueueConsumer(configuration, configuration["RabbitMqRetryQueueName2"]!);
