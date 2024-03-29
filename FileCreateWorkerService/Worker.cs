using ClosedXML.Excel;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Common;
using System.Data;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Product = FileCreateWorkerService.Models.Product;

namespace FileCreateWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private RabbitMQClientService _rabbitmqClientService;
        private readonly IServiceProvider _serviceProvider;  // DbContexti program.cs taraf�na scope olarak ekledi�imiz i�in serviceProvider �zerinden al�yoruz
        private IModel _channel;
        public Worker(ILogger<Worker> logger, RabbitMQClientService rabbitMQClientService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _rabbitmqClientService = rabbitMQClientService;
            _serviceProvider = serviceProvider;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitmqClientService.Connect();
            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientService.queueName, false, consumer);
            consumer.Received += Consumer_Received;
           
            return Task.CompletedTask;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            await Task.Delay(5000);
            var createExcelMessage = JsonSerializer.Deserialize<CreateExcelMessage>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            using var ms = new MemoryStream();
            var wb = new XLWorkbook();
            var ds = new DataSet();
            ds.Tables.Add(GetTable("products"));

            wb.Worksheets.Add(ds);
            wb.SaveAs(ms);

            MultipartFormDataContent multipartFormDataContent = new();
            multipartFormDataContent.Add(new ByteArrayContent(ms.ToArray()), "file", Guid.NewGuid().ToString() + ".xlsx");

            var baseUrl = "https://localhost:7180/api/files";
            using (var httpClient = new HttpClient())
            {

                try
                {
                    var response = await httpClient.PostAsync($"{baseUrl}?fileId={createExcelMessage.FileId}", multipartFormDataContent);
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"File (Id:{createExcelMessage.FileId}) was created by successful");
                        _channel.BasicAck(@event.DeliveryTag, false);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                
            }

        }

        private DataTable GetTable(string tableName)
        {
            List<Product> products;

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                products = context.Products.ToList();
            }
            DataTable table = new DataTable { TableName = tableName };

            table.Columns.Add("ProductID", typeof(int));
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("QuantityPerUnit", typeof(string));
            table.Columns.Add("UnitPrice", typeof(decimal));
            table.Columns.Add("UnitsInStock", typeof(short));
            table.Columns.Add("Discontinued", typeof(bool));

            products.ForEach(x =>
            {
                table.Rows.Add(x.ProductId, x.ProductName, x.QuantityPerUnit, x.UnitPrice, x.UnitsInStock, x.Discontinued);
            });
            return table;
        }
    }
}