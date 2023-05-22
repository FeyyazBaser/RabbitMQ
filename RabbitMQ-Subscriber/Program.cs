
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://lhzvwhvx:6hVBInuK-klWodBsrXQLqyDhIC-tmNuV@rat.rmq2.cloudamqp.com/lhzvwhvx");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

//channel.QueueDeclare("hello-queue", true, false, false); //durable=>rabbitmq refresh olunca kuyruk kalbolmasın memoryde tutmaya yarar
                                                         //exclusive=>true olursa sadece burdaki kanaldan erişebliriz,autodelete=>son subscriber düşerse kuyruk otomatik silinir


var consumer =new EventingBasicConsumer(channel);

channel.BasicConsume("hello-queue", true, consumer);  // autoack=>true olursa kuyruktan siler false yaparsak doğru okunduktan sonra silmesini biz söyleriz

consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Console.WriteLine("Mesajınız: " + message);

};


Console.ReadLine();