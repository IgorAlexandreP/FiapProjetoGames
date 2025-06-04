using System.ComponentModel.DataAnnotations;

namespace FiapProjetoGames.Application.DTOs
{
    public class LoginUsuarioDto
    {
        [Required(ErrorMessage = "O campo E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O E-mail fornecido não é válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório")]
        public string Senha { get; set; }
    }
} 