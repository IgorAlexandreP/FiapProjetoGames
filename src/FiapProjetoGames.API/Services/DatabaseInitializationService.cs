using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FiapProjetoGames.Infrastructure.Data;

namespace FiapProjetoGames.API.Services
{
    public class DatabaseInitializationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInitializationService> _logger;

        public DatabaseInitializationService(IServiceProvider serviceProvider, ILogger<DatabaseInitializationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando inicialização do banco de dados...");

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Verifica se o banco existe e cria se necessário
                await context.Database.EnsureCreatedAsync();

                // Verifica se há dados iniciais
                if (!context.Jogos.Any())
                {
                    _logger.LogInformation("Inserindo dados iniciais...");
                    await SeedInitialDataAsync(context);
                }

                _logger.LogInformation("Banco de dados inicializado com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inicializar banco de dados");
                throw;
            }
        }

        private async Task SeedInitialDataAsync(ApplicationDbContext context)
        {
            // Adiciona alguns jogos de exemplo
            var jogos = new[]
            {
                new Domain.Entities.Jogo
                {
                    Id = Guid.NewGuid(),
                    Titulo = "Pac-Man",
                    Descricao = "Jogo clássico de labirinto",
                    Preco = 19.99m,
                    DataCriacao = DateTime.UtcNow
                },
                new Domain.Entities.Jogo
                {
                    Id = Guid.NewGuid(),
                    Titulo = "Tetris",
                    Descricao = "Jogo de puzzle com blocos",
                    Preco = 14.99m,
                    DataCriacao = DateTime.UtcNow
                },
                new Domain.Entities.Jogo
                {
                    Id = Guid.NewGuid(),
                    Titulo = "Snake",
                    Descricao = "Jogo da cobrinha",
                    Preco = 9.99m,
                    DataCriacao = DateTime.UtcNow
                }
            };

            context.Jogos.AddRange(jogos);
            await context.SaveChangesAsync();

            _logger.LogInformation("Dados iniciais inseridos com sucesso!");
        }
    }
} 