using MassTransit;
using Microsoft.AspNetCore.Mvc;
using ProjetoFiap.Domain.Events;

namespace ProjetoFiap.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public CheckoutController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("simular-pagamento")]
        public async Task<IActionResult> SimularPagamento([FromQuery] decimal valor)
        {
            var pedidoId = Guid.NewGuid();

            // Publica o evento para o RabbitMQ
            await _publishEndpoint.Publish<IProcessarPagamentoEvent>(new
            {
                PedidoId = pedidoId,
                Valor = valor,
                DataSolicitacao = DateTime.UtcNow
            });

            return Accepted(new { 
                Message = "Solicitação de pagamento enviada para processamento assíncrono.", 
                PedidoId = pedidoId,
                Status = "Pendente"
            });
        }
    }
}
