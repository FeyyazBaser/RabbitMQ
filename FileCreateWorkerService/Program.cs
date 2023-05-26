using FileCreateWorkerService;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;





IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(hostContext.Configuration.GetConnectionString("ConnectionString")));
        services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(hostContext.Configuration.GetConnectionString("RabbitMQUri")), DispatchConsumersAsync = true });
        services.AddHostedService<Worker>();
        services.AddSingleton<RabbitMQClientService>();

    }).Build();
await host.RunAsync();

