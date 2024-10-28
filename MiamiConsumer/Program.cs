using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };
var route = Guid.NewGuid();

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "DynamicRouterRules", type: ExchangeType.Direct);

    var message = "Miami|" + route;
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(
        exchange: "DynamicRouterRules",
        routingKey: "",
        basicProperties: null,
        body: body);

    Console.WriteLine($"Rule {message} sent to 'DynamicRouterRules'");
}

factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "direct_bagage_exchange", type: ExchangeType.Direct);

    var queueName = channel.QueueDeclare(queue: "miami_queue", durable: false, exclusive: false, autoDelete: false, arguments: null).QueueName;

    channel.QueueBind(queue: queueName, exchange: "direct_bagage_exchange", routingKey: route.ToString());

    Console.WriteLine(" [*] Waiting for bagage to Miami...");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($" Bagage received: {message}");
    };

    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}