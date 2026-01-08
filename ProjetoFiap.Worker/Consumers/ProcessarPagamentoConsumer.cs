using MassTransit;
using ProjetoFiap.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ProjetoFiap.Worker.Consumers
{
    public class ProcessarPagamentoConsumer : IConsumer<IProcessarPagamentoEvent>
    {
        private readonly ILogger<ProcessarPagamentoConsumer> _logger;

        public ProcessarPagamentoConsumer(ILogger<ProcessarPagamentoConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<IProcessarPagamentoEvent> context)
        {
            _logger.LogInformation("Recebido evento de pagamento: PedidoId={PedidoId}, Valor={Valor}", 
                context.Message.PedidoId, context.Message.Valor);
            
            // Simula processamento
            return Task.CompletedTask;
        }
    }
}
