using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("Hello, World!");

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://localhost:5672");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

//channel.QueueDeclare("hello-queue", true, false, false);

//BasicUsage(channel);

ReceiveMultipleMessage(channel);

Console.ReadLine();




static void BasicUsage(IModel channel)
{
    var consumer = new EventingBasicConsumer(channel);

    channel.BasicConsume("hello-queue", true, consumer);


    consumer.Received += (object sender, BasicDeliverEventArgs e) =>
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        Console.WriteLine("Received Message : " + message);
    };
}

static void ReceiveMultipleMessage(IModel channel)
{
    channel.BasicQos(0, 1, false);

    var consumer = new EventingBasicConsumer(channel);

    channel.BasicConsume("hello-queue", false, consumer);



    consumer.Received += (object sender, BasicDeliverEventArgs e) =>
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());

        Thread.Sleep(1500);
        Console.WriteLine("Gelen Mesaj:" + message);

        channel.BasicAck(e.DeliveryTag, false);
    };
}