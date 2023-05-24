using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Web.Watermark.Services
{
    public class RabbitMQProducer
    {
        private readonly RabbitMQClientService _rabbitmqClientService;
        public RabbitMQProducer(RabbitMQClientService rabbitMQClientService)
        {
              _rabbitmqClientService = rabbitMQClientService;
        }

        public void Publish(ProductImageSavedEvent productImageSavedEvent)
        {
            var channel = _rabbitmqClientService.Connect();
            var bodyString = JsonSerializer.Serialize(productImageSavedEvent);
            var bodyByte=Encoding.UTF8.GetBytes(bodyString);
            var properties =channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(RabbitMQClientService.exchangeName, RabbitMQClientService.watermarkRouteKey, properties, bodyByte);



        }
    }
}
