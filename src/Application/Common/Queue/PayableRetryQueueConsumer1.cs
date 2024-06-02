namespace Application.Common.Queue;

using Microsoft.Extensions.Configuration;

public class PayableRetryQueueConsumer1(IConfiguration configuration)
    : BaseQueueConsumer(configuration, configuration["RabbitMqRetryQueueName1"]!);
