using System;
using System.Text;
using RabbitMQ.Client;

// Sending a bagage to Moscow
SendBagage("Moscow|Bagage: B12345, Destination: Moscow");

// Sending a bagage to Miami
SendBagage("Miami|Bagage: B67890, Destination: Miami");

// Sending a bagage to Miami
SendBagage("Milano|Bagage: B24680, Destination: Milano");


static void SendBagage(string bagageInfo)
{
    var factory = new ConnectionFactory() { HostName = "localhost" };

    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel())
    {

        channel.ExchangeDeclare(exchange: "DynamicRouter", type: ExchangeType.Direct);

        var message = bagageInfo;
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: "DynamicRouter",
            routingKey: "",
            basicProperties: null,
            body: body);

        Console.WriteLine($"Bagage sent '{message}' to 'DynamicRouter'");
    }
}