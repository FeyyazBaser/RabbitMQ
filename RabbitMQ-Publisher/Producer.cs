using RabbitMQ.Client;
using System.Text;



namespace RabbitMQ
{
    public static class Producer
    {

        public static void SendMessageDirectlyQueue()
        {

            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://lhzvwhvx:6hVBInuK-klWodBsrXQLqyDhIC-tmNuV@rat.rmq2.cloudamqp.com/lhzvwhvx");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.QueueDeclare("hello-queue", true, false, false); //durable=>rabbitmq refresh olunca kuyruk kalbolmasın memoryde tutmaya yarar
                                                                     //exclusive=>true olursa sadece burdaki kanaldan erişebliriz,autodelete=>son subscriber düşerse kuyruk otomatik silinir

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"Message {x}";
                var messageBody = Encoding.UTF8.GetBytes(message); // RabbitMQ ya mesajlar byte[] olarak gönderilir

                channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);
                Console.WriteLine($"Mesaj RabbitMQye gönderildi:{message}");
            });

            Console.ReadLine();
        }


        public static void SendMessageWithFanoutExchange()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://lhzvwhvx:6hVBInuK-klWodBsrXQLqyDhIC-tmNuV@rat.rmq2.cloudamqp.com/lhzvwhvx");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"Log {x}";
                var messageBody = Encoding.UTF8.GetBytes(message); // RabbitMQ ya mesajlar byte[] olarak gönderilir

                channel.BasicPublish("logs-fanout", "", null, messageBody);
                Console.WriteLine($"Mesaj RabbitMQye gönderildi:{message}");
            });

            Console.ReadLine();
        }
    }
}
