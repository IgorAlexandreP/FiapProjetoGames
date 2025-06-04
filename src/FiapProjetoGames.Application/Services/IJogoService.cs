using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiapProjetoGames.Application.DTOs;

namespace FiapProjetoGames.Application.Services
{
    public interface IJogoService
    {
        Task<JogoDto> ObterPorIdAsync(Guid id);
        Task<IEnumerable<JogoDto>> ObterTodosAsync();
        Task<JogoDto> CriarAsync(CadastroJogoDto cadastroJogoDto);
        Task AtualizarAsync(Guid id, AtualizacaoJogoDto atualizacaoJogoDto);
        Task DeletarAsync(Guid id);
    }
} 