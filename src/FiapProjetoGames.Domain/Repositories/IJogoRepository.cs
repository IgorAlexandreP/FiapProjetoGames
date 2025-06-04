using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiapProjetoGames.Domain.Entities;

namespace FiapProjetoGames.Domain.Repositories
{
    public interface IJogoRepository
    {
        Task<Jogo> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Jogo>> ObterTodosAsync();
        Task<Jogo> CriarAsync(Jogo jogo);
        Task AtualizarAsync(Jogo jogo);
        Task DeletarAsync(Guid id);
    }
} 