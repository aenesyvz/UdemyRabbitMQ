// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using UdemyRabbitMQ.Shared;

Console.WriteLine("Hello, World!");

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://localhost:5672");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

//DirectExample(channel);

//TopicExample(channel);
HeaderExample(channel);



//FanOutExample(channel);


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

static void DirectExample(IModel channel)
{
    channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);


    Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
    {
        var routeKey = $"route-{x}";
        var queueName = $"direct-queue-{x}";
        channel.QueueDeclare(queueName, true, false, false);

        channel.QueueBind(queueName, "logs-direct", routeKey, null);

    });



    Enumerable.Range(1, 50).ToList().ForEach(x =>
    {

        LogNames log = (LogNames)new Random().Next(1, 5);

        string message = $"log-type: {log}";

        var messageBody = Encoding.UTF8.GetBytes(message);

        var routeKey = $"route-{log}";

        channel.BasicPublish("logs-direct", routeKey, null, messageBody);

        Console.WriteLine($"Log gönderilmiştir : {message}");

    });
}

static void TopicExample(IModel channel)
{
    channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);

    Random rnd = new Random();
    Enumerable.Range(1, 50).ToList().ForEach(x =>
    {

        LogNames log1 = (LogNames)rnd.Next(1, 5);
        LogNames log2 = (LogNames)rnd.Next(1, 5);
        LogNames log3 = (LogNames)rnd.Next(1, 5);

        var routeKey = $"{log1}.{log2}.{log3}";
        string message = $"log-type: {log1}-{log2}-{log3}";
        var messageBody = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish("logs-topic", routeKey, null, messageBody);

        Console.WriteLine($"Log gönderilmiştir : {message}");

    });
}

static void HeaderExample(IModel channel)
{
    channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

    Dictionary<string, object> headers = new Dictionary<string, object>();

    headers.Add("format", "pdf");
    headers.Add("shape2", "a4");

    var properties = channel.CreateBasicProperties();
    properties.Headers = headers;

    //properties.Persistent = true;   ->  Mesajları kalıcı hale getirme

    //var product = new Product() { Id = 1, Name="Kalem",Price=100,Stock=10};
    //var productJsonString = JsonSerializer.Serialize(product);

    channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("header mesajım"));

    Console.WriteLine("mesaj gönderilmiştir");
}