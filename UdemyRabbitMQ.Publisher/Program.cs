// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using System.Text;

Console.WriteLine("Hello, World!");

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://localhost:5672");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

FanOutExample(channel);


//  ---------------------- BasicUsage - SendMultiple   ----------------------------


//channel.QueueDeclare("hello-queue", true, false, false);

//BasicUsage(channel);

//SendMultipleMessages(channel);

Console.ReadLine();







static void BasicUsage(IModel channel)
{
    string message = "hello world";

    var messageBody = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

    Console.WriteLine("Send Message!");
}


static void SendMultipleMessages(IModel channel)
{
    Enumerable.Range(1, 50).ToList().ForEach(x =>
    {

        string message = $"Message {x}";

        var messageBody = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

        Console.WriteLine($"Mesaj gönderilmiştir : {message}");

    });
}

static void FanOutExample(IModel channel)
{
    channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

    Enumerable.Range(1, 50).ToList().ForEach(x =>
    {

        string message = $"log {x}";

        var messageBody = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish("logs-fanout", "", null, messageBody);

        Console.WriteLine($"Mesaj gönderilmiştir : {message}");

    });
}