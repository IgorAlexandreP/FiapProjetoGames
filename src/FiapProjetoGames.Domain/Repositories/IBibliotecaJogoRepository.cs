using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiapProjetoGames.Domain.Entities;

namespace FiapProjetoGames.Domain.Repositories
{
    public interface IBibliotecaJogoRepository
    {
        Task<BibliotecaJogo> ObterPorIdAsync(Guid id);
        Task<IEnumerable<BibliotecaJogo>> ObterPorUsuarioIdAsync(Guid usuarioId);
        Task<BibliotecaJogo> CriarAsync(BibliotecaJogo bibliotecaJogo);
        Task<bool> UsuarioPossuiJogoAsync(Guid usuarioId, Guid jogoId);
        Task DeletarAsync(Guid id);
        Task<BibliotecaJogo> ObterPorUsuarioEJogoIdAsync(Guid usuarioId, Guid jogoId);
    }
} 