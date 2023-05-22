
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://lhzvwhvx:6hVBInuK-klWodBsrXQLqyDhIC-tmNuV@rat.rmq2.cloudamqp.com/lhzvwhvx");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

//channel.QueueDeclare("hello-queue", true, false, false); //durable=>rabbitmq refresh olunca kuyruk kalbolmasın memoryde tutmaya yarar
//exclusive=>true olursa sadece burdaki kanaldan erişebliriz,autodelete=>son subscriber düşerse kuyruk otomatik silinir

channel.BasicQos(0, 1, false); //global=>true olursa prefetchCount sayısı subscriberlara paylaştırır false olursa her birine prefetchCount kadar mesaj gönderir

var consumer =new EventingBasicConsumer(channel);

channel.BasicConsume("hello-queue", false, consumer);  // autoack=>true olursa kuyruktan siler false yaparsak doğru okunduktan sonra silmesini biz söyleriz

consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Console.WriteLine("Mesajınız: " + message);
    channel.BasicAck(e.DeliveryTag, false);
       Thread.Sleep(1500);
};


Console.ReadLine();