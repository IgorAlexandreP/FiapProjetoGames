using System;

namespace FiapProjetoGames.Application.DTOs
{
    public class UsuarioDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public string Token { get; set; }
    }

    public class AtualizacaoUsuarioDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
    }

    public class AtualizacaoSenhaDto
    {
        public string SenhaAtual { get; set; }
        public string NovaSenha { get; set; }
    }
} 