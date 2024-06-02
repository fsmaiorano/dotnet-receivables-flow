using Microsoft.Extensions.Configuration;

namespace Application.Common.Queue;

public class PayableQueueConsumer(IConfiguration configuration)
    : BaseQueueConsumer(configuration, configuration["RabbitMqQueueName"]!);
