using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;

    public RabbitMqConsumer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };

        var connection = await factory.CreateConnectionAsync();

        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "order-created",
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, args) =>
        {
            var json =
                Encoding.UTF8.GetString(args.Body.ToArray());

            Console.WriteLine(
                $"Order Created Event: {json}");

            await channel.BasicAckAsync(
                args.DeliveryTag,
                multiple: false);
        };

        await channel.BasicConsumeAsync(
            queue: "order-created",
            autoAck: false,
            consumer: consumer);

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }
}