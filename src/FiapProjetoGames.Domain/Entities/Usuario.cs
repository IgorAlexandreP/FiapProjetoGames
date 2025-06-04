using System;

namespace FiapProjetoGames.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string SenhaHash { get; private set; }
        public bool IsAdmin { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }

        public Usuario(string nome, string email, string senhaHash, bool isAdmin = false)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Email = email;
            SenhaHash = senhaHash;
            IsAdmin = isAdmin;
            DataCriacao = DateTime.UtcNow;
        }

        public void AtualizarDados(string nome, string email)
        {
            if (!string.IsNullOrEmpty(nome))
                Nome = nome;
            
            if (!string.IsNullOrEmpty(email))
                Email = email;

            DataAtualizacao = DateTime.UtcNow;
        }

        public void AtualizarSenha(string novaSenhaHash)
        {
            SenhaHash = novaSenhaHash;
            DataAtualizacao = DateTime.UtcNow;
        }
    }
} 