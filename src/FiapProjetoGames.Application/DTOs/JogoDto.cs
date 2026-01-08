using System;

namespace FiapProjetoGames.Application.DTOs
{
    public class JogoDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
    }

    public class CadastroJogoDto
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
    }

    public class AtualizacaoJogoDto
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
    }
} 