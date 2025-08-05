using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FiapProjetoGames.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Erro não tratado: {Message}", exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case InvalidOperationException invalidOpEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new ErrorResponse
                    {
                        Message = GetUserFriendlyMessage(invalidOpEx.Message),
                        Type = "ValidationError",
                        Details = GetUserFriendlyDetails(invalidOpEx.Message)
                    };
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse = new ErrorResponse
                    {
                        Message = "Acesso negado. Verifique suas credenciais.",
                        Type = "AuthenticationError"
                    };
                    break;

                case ArgumentException argEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new ErrorResponse
                    {
                        Message = "Dados inválidos fornecidos.",
                        Type = "ValidationError",
                        Details = GetUserFriendlyDetails(argEx.Message)
                    };
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new ErrorResponse
                    {
                        Message = "Ocorreu um erro interno. Tente novamente mais tarde.",
                        Type = "InternalServerError"
                    };
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(result);
        }

        private string GetUserFriendlyMessage(string technicalMessage)
        {
            return technicalMessage.ToLower() switch
            {
                var msg when msg.Contains("email já está em uso") => "Este email já está cadastrado em nossa plataforma.",
                var msg when msg.Contains("email ou senha inválidos") => "Email ou senha incorretos. Verifique suas credenciais.",
                var msg when msg.Contains("conta inativa") => "Sua conta está inativa. Entre em contato com o suporte.",
                var msg when msg.Contains("conta temporariamente bloqueada") => "Sua conta está temporariamente bloqueada devido a múltiplas tentativas de login.",
                var msg when msg.Contains("senha atual incorreta") => "A senha atual informada está incorreta.",
                var msg when msg.Contains("usuário não encontrado") => "Usuário não encontrado.",
                var msg when msg.Contains("email já existente") => "Este email já está sendo usado por outro usuário.",
                _ => "Ocorreu um erro. Tente novamente."
            };
        }

        private string GetUserFriendlyDetails(string technicalMessage)
        {
            return technicalMessage.ToLower() switch
            {
                var msg when msg.Contains("email já está em uso") => "Tente fazer login com este email ou use um email diferente para o cadastro.",
                var msg when msg.Contains("email ou senha inválidos") => "Verifique se digitou corretamente seu email e senha.",
                var msg when msg.Contains("conta inativa") => "Entre em contato com nossa equipe de suporte para reativar sua conta.",
                var msg when msg.Contains("conta temporariamente bloqueada") => "Aguarde alguns minutos antes de tentar novamente.",
                var msg when msg.Contains("senha atual incorreta") => "Certifique-se de que digitou sua senha atual corretamente.",
                var msg when msg.Contains("usuário não encontrado") => "Verifique se o ID do usuário está correto.",
                var msg when msg.Contains("email já existente") => "Use um email diferente ou faça login com este email.",
                _ => "Se o problema persistir, entre em contato com o suporte."
            };
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
} 