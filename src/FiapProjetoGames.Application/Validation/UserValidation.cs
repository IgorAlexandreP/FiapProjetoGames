using System;
using System.Text.RegularExpressions;

namespace FiapCloudGames.Application.Validation
{
    public static class UserValidation
    {
        public static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O e-mail é obrigatório.");

            // Regex para validar formato de e-mail
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
                throw new ArgumentException("O formato do e-mail é inválido.");
        }

        public static void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("A senha é obrigatória.");

            if (password.Length < 8)
                throw new ArgumentException("A senha deve ter no mínimo 8 caracteres.");

            // Verifica se contém pelo menos um número
            if (!password.Any(char.IsDigit))
                throw new ArgumentException("A senha deve conter pelo menos um número.");

            // Verifica se contém pelo menos uma letra minúscula
            if (!password.Any(char.IsLower))
                throw new ArgumentException("A senha deve conter pelo menos uma letra minúscula.");

            // Verifica se contém pelo menos uma letra maiúscula
            if (!password.Any(char.IsUpper))
                throw new ArgumentException("A senha deve conter pelo menos uma letra maiúscula.");

            // Verifica se contém pelo menos um caractere especial
            var specialChars = @"!@#$%^&*()_+-=[]{}|;:,.<>?";
            if (!password.Any(c => specialChars.Contains(c)))
                throw new ArgumentException("A senha deve conter pelo menos um caractere especial (!@#$%^&*()_+-=[]{}|;:,.<>?)");
        }

        public static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome é obrigatório.");

            if (name.Length < 3)
                throw new ArgumentException("O nome deve ter no mínimo 3 caracteres.");
        }
    }
} 