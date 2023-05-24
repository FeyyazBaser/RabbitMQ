using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Web.Watermark.Services;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Web.Watermark.BackgroundServices
{
    public class ImageWatermarkBackgroundService : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private readonly ILogger<ImageWatermarkBackgroundService> _logger;
        private IModel _channel;
        public ImageWatermarkBackgroundService(RabbitMQClientService rabbitMQClientService, ILogger<ImageWatermarkBackgroundService> logger)
        {
            _rabbitMQClientService = rabbitMQClientService;
            _logger = logger;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer =new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientService.queueName, false, consumer);
            consumer.Received += Consumer_Received;
            
            return Task.CompletedTask;

        }

        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                var imageCretedEvent = JsonSerializer.Deserialize<ProductImageSavedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imageCretedEvent.ImageName);
                var watermark = "www.watermarks.com";
                using var img = Image.FromFile(path);
                using var graphic = Graphics.FromImage(img);

                var font = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Bold, GraphicsUnit.Pixel);

                var textSize = graphic.MeasureString(watermark, font);

                var color = Color.White;

                var brush = new SolidBrush(color);
                var position = new Point(img.Width - ((int)textSize.Width + 30), img.Height - ((int)textSize.Height + 30));

                graphic.DrawString(watermark, font, brush, position);
                img.Save("wwwroot/watermarks/" + imageCretedEvent.ImageName);
                img.Dispose();
                graphic.Dispose();
                _channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
            }
            return Task.CompletedTask;

        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
