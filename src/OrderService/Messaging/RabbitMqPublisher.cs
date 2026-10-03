using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace OrderService.Messaging;

public class RabbitMqPublisher
{
    private readonly IConfiguration _configuration;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishAsync<T>(T message, string queueName)
    {
        var host = _configuration["RabbitMQ:Host"] ?? "localhost";

        var factory = new ConnectionFactory
        {
            HostName = host,
            UserName = "guest",
            Password = "guest"
        };

        await using var connection =
            await factory.CreateConnectionAsync();

        await using var channel =
            await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        var json = JsonSerializer.Serialize(message);

        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            body: body);
    }
}