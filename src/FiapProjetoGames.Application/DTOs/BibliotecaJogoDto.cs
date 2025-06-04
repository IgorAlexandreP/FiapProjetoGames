using System;

namespace FiapProjetoGames.Application.DTOs
{
    public class BibliotecaJogoDto
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public Guid JogoId { get; set; }
        public DateTime DataCompra { get; set; }
        public decimal PrecoCompra { get; set; }
        public JogoDto Jogo { get; set; }
    }
} 