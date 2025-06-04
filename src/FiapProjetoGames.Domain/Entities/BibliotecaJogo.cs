using System;

namespace FiapProjetoGames.Domain.Entities
{
    public class BibliotecaJogo
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Guid JogoId { get; private set; }
        public DateTime DataCompra { get; private set; }
        public decimal PrecoCompra { get; private set; }
        public virtual Jogo? Jogo { get; private set; }

        public BibliotecaJogo(Guid usuarioId, Guid jogoId, decimal precoCompra)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            JogoId = jogoId;
            DataCompra = DateTime.UtcNow;
            PrecoCompra = precoCompra;
        }
    }
} 