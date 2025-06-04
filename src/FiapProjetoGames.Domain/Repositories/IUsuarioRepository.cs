using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiapProjetoGames.Domain.Entities;

namespace FiapProjetoGames.Domain.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterPorIdAsync(Guid id);
        Task<Usuario> ObterPorEmailAsync(string email);
        Task<IEnumerable<Usuario>> ObterTodosAsync();
        Task<Usuario> CriarAsync(Usuario usuario);
        Task AtualizarAsync(Usuario usuario);
        Task DeletarAsync(Guid id);
    }
} 