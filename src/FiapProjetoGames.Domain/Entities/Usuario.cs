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
        public bool Ativo { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }
        public DateTime? UltimoLogin { get; private set; }
        public int TentativasLogin { get; private set; }
        public DateTime? BloqueadoAte { get; private set; }

        public Usuario(string nome, string email, string senhaHash, bool isAdmin = false)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Email = email;
            SenhaHash = senhaHash;
            IsAdmin = isAdmin;
            Ativo = true;
            DataCriacao = DateTime.UtcNow;
            TentativasLogin = 0;
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
            TentativasLogin = 0;
            BloqueadoAte = null;
        }

        public void RegistrarLogin()
        {
            UltimoLogin = DateTime.UtcNow;
            TentativasLogin = 0;
            BloqueadoAte = null;
        }

        public void IncrementarTentativaLogin()
        {
            TentativasLogin++;
            if (TentativasLogin >= 5)
            {
                BloqueadoAte = DateTime.UtcNow.AddMinutes(30);
            }
        }

        public void LoginBemSucedido()
        {
            UltimoLogin = DateTime.UtcNow;
            TentativasLogin = 0;
            BloqueadoAte = null;
        }

        public void Bloquear(int minutos = 30)
        {
            BloqueadoAte = DateTime.UtcNow.AddMinutes(minutos);
            DataAtualizacao = DateTime.UtcNow;
        }

        public void Desbloquear()
        {
            BloqueadoAte = null;
            TentativasLogin = 0;
            DataAtualizacao = DateTime.UtcNow;
        }

        public bool EstaBloqueado()
        {
            return BloqueadoAte.HasValue && BloqueadoAte.Value > DateTime.UtcNow;
        }

        public void Ativar()
        {
            Ativo = true;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void Desativar()
        {
            Ativo = false;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void TornarAdmin()
        {
            IsAdmin = true;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void RemoverAdmin()
        {
            IsAdmin = false;
            DataAtualizacao = DateTime.UtcNow;
        }
    }
} 