using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace FiapProjetoGames.Application.DTOs
{
    [SwaggerSchema(Description = "DTO para cadastro de novo usuário")]
    public class CadastroUsuarioDto
    {
        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [MinLength(3, ErrorMessage = "O Nome deve ter pelo menos 3 caracteres")]
        [Description("Nome do usuário")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O E-mail fornecido não é válido")]
        [Description("E-mail do usuário")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Senha é obrigatório")]
        [MinLength(8, ErrorMessage = "A Senha deve ter pelo menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
            ErrorMessage = "A Senha deve conter pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial")]
        [Description("Senha do usuário (mínimo 8 caracteres, deve conter maiúscula, minúscula, número e caractere especial)")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Confirmar Senha é obrigatório")]
        [Compare("Senha", ErrorMessage = "A confirmação de senha deve ser igual à senha")]
        [Description("Confirmação da senha")]
        public string ConfirmarSenha { get; set; } = string.Empty;

        [Description("Define se o usuário é administrador")]
        public bool IsAdmin { get; set; }
    }
} 