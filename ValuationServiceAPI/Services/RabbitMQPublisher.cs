using System.Text.Json;
using RabbitMQ.Client;
using ValuationServiceAPI.Models;
using Microsoft.Extensions.Logging;

namespace ValuationServiceAPI.Services
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly ILogger<RabbitMqPublisher> _logger;

        public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger)
        {
            _logger = logger;
        }

        public async Task PublishAsync(ItemAssessmentDTO dto)
        {
            var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            var factory = new ConnectionFactory { HostName = host };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "assessmentQueue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var body = JsonSerializer.SerializeToUtf8Bytes(dto);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "assessmentQueue",
                mandatory: false,
                basicProperties: new BasicProperties(),
                body: body
            );

            _logger.LogInformation("Published assessment with ID {id}", dto.Effect.Id);

        }
    }
}
