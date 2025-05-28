using System.Text.Json;
using RabbitMQ.Client;
using ValuationServiceAPI.Models;
using Microsoft.Extensions.Logging;

namespace ValuationServiceAPI.Services
{
    /// <summary>
    /// Service til at publicere ItemAssessmentDTO-objekter til en RabbitMQ-kø.
    /// </summary>
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly ILogger<RabbitMqPublisher> _logger;

        /// <summary>
        /// Initialiserer en ny instans af RabbitMqPublisher med logger.
        /// </summary>
        /// <param name="logger">Logger til informations- og fejlhåndtering.</param>
        public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sender en ItemAssessmentDTO til RabbitMQ-køen "assessmentQueue" asynkront.
        /// </summary>
        /// <param name="dto">Dataobjektet, der skal publiceres.</param>
        /// <returns>En task, der repræsenterer den asynkrone operation.</returns>
        public async Task PublishAsync(ItemAssessmentDTO dto)
        {
            var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            var factory = new ConnectionFactory { HostName = host };

            int retries = 10;
            Exception? lastException = null;

            for (int i = 1; i <= retries; i++)
            {
                try
                {
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

                    _logger.LogInformation("✔ Published assessment: {@Assessment}", dto);
                    return;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.LogWarning(ex, "RabbitMQ ikke klar (forsøg {Attempt}/{Retries}). Prøver igen...", i, retries);
                    await Task.Delay(3000);
                }
            }

            _logger.LogError("Kunne ikke publicere til RabbitMQ efter {Retries} forsøg.", retries);
            throw lastException!; // <-- kritisk! kast videre til SeedData så den ved det fejlede
        }
    }
}
