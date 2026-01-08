using System;

namespace FiapProjetoGames.Domain.Entities
{
    public class Jogo
    {
        public Guid Id { get; private set; }
        public string Titulo { get; private set; }
        public string Descricao { get; private set; }
        public decimal Preco { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }

        public Jogo(string titulo, string descricao, decimal preco)
        {
            Id = Guid.NewGuid();
            Titulo = titulo;
            Descricao = descricao;
            Preco = preco;
            DataCriacao = DateTime.UtcNow;
        }

        public void Atualizar(string titulo, string descricao, decimal preco)
        {
            if (!string.IsNullOrEmpty(titulo))
                Titulo = titulo;
            
            if (!string.IsNullOrEmpty(descricao))
                Descricao = descricao;
            
            if (preco > 0)
                Preco = preco;

            DataAtualizacao = DateTime.UtcNow;
        }
    }
} 