using MassTransit;
using ProjetoFiap.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProcessarPagamentoConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq") ?? "amqp://guest:guest@localhost:5672");
        
        cfg.ReceiveEndpoint("processar-pagamento-queue", e =>
        {
            e.ConfigureConsumer<ProcessarPagamentoConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
