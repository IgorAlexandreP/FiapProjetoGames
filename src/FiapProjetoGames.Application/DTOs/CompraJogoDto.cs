using System;
using System.ComponentModel.DataAnnotations;

namespace FiapProjetoGames.Application.DTOs
{
    public class CompraJogoDto
    {
        [Required(ErrorMessage = "O ID do jogo é obrigatório")]
        public Guid JogoId { get; set; }

        [Required(ErrorMessage = "O preço de compra é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço de compra deve ser maior que zero")]
        public decimal PrecoCompra { get; set; }
    }
} 