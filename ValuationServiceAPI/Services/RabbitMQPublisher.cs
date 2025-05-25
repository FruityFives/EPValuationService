using System.Text.Json;
using RabbitMQ.Client;
using ValuationServiceAPI.Models;
using Microsoft.Extensions.Logging;

namespace ValuationServiceAPI.Services;

/// <summary>
/// Publisher der sender ItemAssessmentDTO-objekter til RabbitMQ.
/// </summary>
public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Publicerer et vurderingsobjekt til RabbitMQ-køen "assessmentQueue".
    /// </summary>
    /// <param name="dto">Det DTO-objekt der skal sendes.</param>
    public async Task PublishAsync(ItemAssessmentDTO dto)
    {
        var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        var factory = new ConnectionFactory { HostName = host };

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

            _logger.LogInformation("AssessmentDTO publiceret til RabbitMQ: {@DTO}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fejl ved publicering til RabbitMQ");
            throw; // kast videre til evt. global error handler
        }
    }
}
