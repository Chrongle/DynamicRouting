using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

var RuleDictionary = new Dictionary<string, string>();

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{

    ListenToDynamicRouterRules(channel);
    ListenToDynamicRouter(channel);

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}

void ListenToDynamicRouterRules(IModel channel)
{
    channel.ExchangeDeclare(exchange: "DynamicRouterRules", type: ExchangeType.Direct);

    var queueName = channel.QueueDeclare(queue: "DynamicRouterRules", durable: false, exclusive: false, autoDelete: false, arguments: null).QueueName;
    channel.QueueBind(queue: queueName, exchange: "DynamicRouterRules", routingKey: "");

    Console.WriteLine(" [*] Waiting for rule to DynamicRouterRules...");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($" rule received: {message}");

        var rule = message.Split('|');
        RuleDictionary[rule[0]] = rule[1];
    };

    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
}

void ListenToDynamicRouter(IModel channel)
{

    channel.ExchangeDeclare(exchange: "DynamicRouter", type: ExchangeType.Direct);

    var queueName = channel.QueueDeclare(queue: "DynamicRouter", durable: false, exclusive: false, autoDelete: false, arguments: null).QueueName;
    channel.QueueBind(queue: queueName, exchange: "DynamicRouter", routingKey: "");

    Console.WriteLine(" [*] Waiting for bagage to DynamicRouter...");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($" Bagage received: {message}");

        var properties = message.Split('|');
        SendMessage(properties);
    };

    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

}

void SendMessage(string[] properties)
{
    var factory = new ConnectionFactory() { HostName = "localhost" };

    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel())
    {

        channel.ExchangeDeclare(exchange: "direct_bagage_exchange", type: ExchangeType.Direct);


        var routingKey = DetermineRoutingKey(properties[0]);
        var message = properties[1];
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: "direct_bagage_exchange",
            routingKey: routingKey,
            basicProperties: null,
            body: body);

        Console.WriteLine($"Bagage sent '{message}' to '{routingKey}'");
    }
}

string DetermineRoutingKey(string destination)
{
    if (RuleDictionary.TryGetValue(destination, out var route))
    {
        return route;
    }
    else
    {
        Console.WriteLine($"Warning: No route found for destination '{destination}'");
        return "default";
    }
}