namespace Application.Common.Queue;

using Microsoft.Extensions.Configuration;

public class PayableRetryQueueConsumer4(IConfiguration configuration)
    : BaseQueueConsumer(configuration, configuration["RabbitMqRetryQueueName4"]!);
