using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ_Common;
using System.Text.Json;

namespace RabbitMQ
{
    public static class Consumer
    {
        public static void GetDirectlyMessageFromQueue()
        {

            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://lhzvwhvx:6hVBInuK-klWodBsrXQLqyDhIC-tmNuV@rat.rmq2.cloudamqp.com/lhzvwhvx");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            //channel.QueueDeclare("hello-queue", true, false, false); //durable=>rabbitmq refresh olunca kuyruk kalbolmasın memoryde tutmaya yarar
            //exclusive=>true olursa sadece burdaki kanaldan erişebliriz,autodelete=>son subscriber düşerse kuyruk otomatik silinir

            channel.BasicQos(0, 1, false); //global=>true olursa prefetchCount sayısı subscriberlara paylaştırır false olursa her birine prefetchCount kadar mesaj gönderir

            var consumer = new EventingBasicConsumer(channel);



            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("Mesajınız: " + message);
                channel.BasicAck(e.DeliveryTag, false);

            };

            channel.BasicConsume("hello-queue", false, consumer);  // autoack=>true olursa kuyruktan siler false yaparsak doğru okunduktan sonra silmesini biz söyleriz
            Console.ReadLine();
        }

        public static void GetMessageWithFanoutExchangeFromQueue()
        {

            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://lhzvwhvx:6hVBInuK-klWodBsrXQLqyDhIC-tmNuV@rat.rmq2.cloudamqp.com/lhzvwhvx");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            var randomQueueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(randomQueueName, "logs-fanout", "", null); // Uygulama ayağa kalktığında kuyruk oluşacak kapandığında kuyruk silinecek


            //channel.QueueDeclare("hello-queue", true, false, false); //durable=>rabbitmq refresh olunca kuyruk kalbolmasın memoryde tutmaya yarar
            //exclusive=>true olursa sadece burdaki kanaldan erişebliriz,autodelete=>son subscriber düşerse kuyruk otomatik silinir

            channel.BasicQos(0, 1, false); //global=>true olursa prefetchCount sayısı subscriberlara paylaştırır false olursa her birine prefetchCount kadar mesaj gönderir

            var consumer = new EventingBasicConsumer(channel);



            Console.WriteLine("Loglar dinleniyor...");
            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("Mesajınız: " + message);
                channel.BasicAck(e.DeliveryTag, false);

            };

            channel.BasicConsume(randomQueueName, false, consumer);  // autoack=>true olursa kuyruktan siler false yaparsak doğru okunduktan sonra silmesini biz söyleriz
            Console.ReadLine();
        }

        public static void GetMessageWithDirectExchangeFromQueue()
        {

            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://lhzvwhvx:6hVBInuK-klWodBsrXQLqyDhIC-tmNuV@rat.rmq2.cloudamqp.com/lhzvwhvx");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.BasicQos(0, 1, false); //global=>true olursa prefetchCount sayısı subscriberlara paylaştırır false olursa her birine prefetchCount kadar mesaj gönderir

            var consumer = new EventingBasicConsumer(channel);
            var queueName = "direct-queue-Info";


            Console.WriteLine("Loglar dinleniyor...");
            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("Mesajınız: " + message);
                //File.AppendAllText("log-critical.txt", message + "\n");

                channel.BasicAck(e.DeliveryTag, false);


            };
            channel.BasicConsume(queueName, false, consumer);  // autoack=>true olursa kuyruktan siler false yaparsak doğru okunduktan sonra silmesini biz söyleriz


            Console.ReadLine();
        }
        public static void GetMessageWithTopicExchangeFromQueue()
        {

            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://lhzvwhvx:6hVBInuK-klWodBsrXQLqyDhIC-tmNuV@rat.rmq2.cloudamqp.com/lhzvwhvx");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.BasicQos(0, 1, false); //global=>true olursa prefetchCount sayısı subscriberlara paylaştırır false olursa her birine prefetchCount kadar mesaj gönderir

            var consumer = new EventingBasicConsumer(channel);
            var randomQueueName = channel.QueueDeclare().QueueName;

            var routeKey = "#.Info";
            channel.QueueBind(randomQueueName, "logs-topic", routeKey);


            Console.WriteLine("Loglar dinleniyor...");
            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("Mesajınız: " + message);
                //File.AppendAllText("log-critical.txt", message + "\n");

                channel.BasicAck(e.DeliveryTag, false);


            };
            channel.BasicConsume(randomQueueName, false, consumer);  // autoack=>true olursa kuyruktan siler false yaparsak doğru okunduktan sonra silmesini biz söyleriz

            Console.ReadLine();
        }

        public static void GetMessageWithHeaderExchangeFromQueue()
        {

            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://lhzvwhvx:6hVBInuK-klWodBsrXQLqyDhIC-tmNuV@rat.rmq2.cloudamqp.com/lhzvwhvx");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.BasicQos(0, 1, false); //global=>true olursa prefetchCount sayısı subscriberlara paylaştırır false olursa her birine prefetchCount kadar mesaj gönderir

            var consumer = new EventingBasicConsumer(channel);
            var randomQueueName = channel.QueueDeclare().QueueName;

            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("fromat", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match", "all");

            channel.QueueBind(randomQueueName, "header-exchange", "", headers);


            Console.WriteLine("Loglar dinleniyor...");
            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("Mesajınız: " + message);


                channel.BasicAck(e.DeliveryTag, false);


            };
            channel.BasicConsume(randomQueueName, false, consumer);  // autoack=>true olursa kuyruktan siler false yaparsak doğru okunduktan sonra silmesini biz söyleriz

            Console.ReadLine();
        }

        public static void GetComplexTypeWithHeaderExchangeFromQueue()
        {

            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://lhzvwhvx:6hVBInuK-klWodBsrXQLqyDhIC-tmNuV@rat.rmq2.cloudamqp.com/lhzvwhvx");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.BasicQos(0, 1, false); //global=>true olursa prefetchCount sayısı subscriberlara paylaştırır false olursa her birine prefetchCount kadar mesaj gönderir

            var consumer = new EventingBasicConsumer(channel);
            var randomQueueName = channel.QueueDeclare().QueueName;

            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("fromat", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match", "any");

            channel.QueueBind(randomQueueName, "header-exchange", "", headers);


            Console.WriteLine("Loglar dinleniyor...");
            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Product product = JsonSerializer.Deserialize<Product>(message);
                Thread.Sleep(1500);
                Console.WriteLine($"Mesajınız: {product.Id}-{product.Name}-{product.Price}-{product.Stock}");


                channel.BasicAck(e.DeliveryTag, false);


            };
            channel.BasicConsume(randomQueueName, false, consumer);  // autoack=>true olursa kuyruktan siler false yaparsak doğru okunduktan sonra silmesini biz söyleriz

            Console.ReadLine();
        }


    }
}
