namespace ProjetoFiap.Domain.Events
{
    public interface IProcessarPagamentoEvent
    {
        Guid PedidoId { get; }
        decimal Valor { get; }
        DateTime DataSolicitacao { get; }
    }
}
