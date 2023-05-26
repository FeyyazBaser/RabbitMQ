using RabbitMQ.Client;
using RabbitMQ_Common;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Web.Excel.Services
{
    public class RabbitMQProducer
    {
        private readonly RabbitMQClientService _rabbitmqClientService;
        public RabbitMQProducer(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitmqClientService = rabbitMQClientService;
        }

        public void Publish(CreateExcelMessage createExcelMessage)
        {
            var channel = _rabbitmqClientService.Connect();
            var bodyString = JsonSerializer.Serialize(createExcelMessage);
            var bodyByte = Encoding.UTF8.GetBytes(bodyString);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(RabbitMQClientService.exchangeName, RabbitMQClientService.excelRouteKey, properties, bodyByte);

        }
    }
}
