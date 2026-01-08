using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Swashbuckle.AspNetCore.Annotations;

namespace FiapProjetoGames.Application.DTOs
{
    [SwaggerSchema(Description = "DTO para login de usuário")]
    public class LoginUsuarioDto
    {
        [Required(ErrorMessage = "O campo E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O E-mail fornecido não é válido")]
        [Description("E-mail do usuário")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Senha é obrigatório")]
        [Description("Senha do usuário")]
        public string Senha { get; set; } = string.Empty;
    }
} 