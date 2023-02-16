using BLL.QueueServices;
using DAL.Context;
using EXCEL_WS;
using RabbitMQ.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTransient<ConnectionFactory>(x => new ConnectionFactory
        {
            Uri = new Uri(hostContext.Configuration.GetConnectionString("RabbitMQ")),
            DispatchConsumersAsync= true
        });
        services.AddTransient<RabbitMQClientService>();
        services.AddTransient<RabbitMQClientPublisher>();
        services.AddTransient<RabbitMQClientSubscriber>();

        services.AddDbContext<AppDbContext>();

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
