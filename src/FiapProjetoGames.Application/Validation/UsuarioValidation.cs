using System;
using System.Text.RegularExpressions;
using FluentValidation;
using FiapProjetoGames.Application.DTOs;

namespace FiapProjetoGames.Application.Validation
{
    public class CadastroUsuarioValidation : AbstractValidator<CadastroUsuarioDto>
    {
        public CadastroUsuarioValidation()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .Length(2, 100).WithMessage("Nome deve ter entre 2 e 100 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras e espaços");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email deve ser válido")
                .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres")
                .MaximumLength(128).WithMessage("Senha deve ter no máximo 128 caracteres")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("Senha deve conter pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial");

            RuleFor(x => x.ConfirmarSenha)
                .Equal(x => x.Senha).WithMessage("Confirmação de senha deve ser igual à senha");
        }
    }

    public class LoginUsuarioValidation : AbstractValidator<LoginUsuarioDto>
    {
        public LoginUsuarioValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email deve ser válido");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória");
        }
    }

    public class AtualizacaoUsuarioValidation : AbstractValidator<AtualizacaoUsuarioDto>
    {
        public AtualizacaoUsuarioValidation()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .Length(2, 100).WithMessage("Nome deve ter entre 2 e 100 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras e espaços");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email deve ser válido")
                .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres");
        }
    }

    public class AtualizacaoSenhaValidation : AbstractValidator<AtualizacaoSenhaDto>
    {
        public AtualizacaoSenhaValidation()
        {
            RuleFor(x => x.SenhaAtual)
                .NotEmpty().WithMessage("Senha atual é obrigatória");

            RuleFor(x => x.NovaSenha)
                .NotEmpty().WithMessage("Nova senha é obrigatória")
                .MinimumLength(8).WithMessage("Nova senha deve ter no mínimo 8 caracteres")
                .MaximumLength(128).WithMessage("Nova senha deve ter no máximo 128 caracteres")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("Nova senha deve conter pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial")
                .NotEqual(x => x.SenhaAtual).WithMessage("Nova senha deve ser diferente da senha atual");
        }
    }

    public class UsuarioFiltroValidation : AbstractValidator<UsuarioFiltroDto>
    {
        public UsuarioFiltroValidation()
        {
            RuleFor(x => x.Nome)
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

            RuleFor(x => x.Email)
                .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres");

            RuleFor(x => x.Pagina)
                .GreaterThan(0).WithMessage("Página deve ser maior que 0");

            RuleFor(x => x.TamanhoPagina)
                .InclusiveBetween(1, 100).WithMessage("Tamanho da página deve estar entre 1 e 100");

            RuleFor(x => x.DataCriacaoInicio)
                .LessThanOrEqualTo(x => x.DataCriacaoFim)
                .When(x => x.DataCriacaoInicio.HasValue && x.DataCriacaoFim.HasValue)
                .WithMessage("Data de início deve ser menor ou igual à data de fim");
        }
    }
} 