using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiapProjetoGames.Application.DTOs;

namespace FiapProjetoGames.Application.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioDto> CadastrarAsync(CadastroUsuarioDto cadastroUsuarioDto);
        Task<UsuarioDto> LoginAsync(LoginUsuarioDto loginUsuarioDto);
        Task<UsuarioDto> ObterPorIdAsync(Guid id);
        Task<IEnumerable<UsuarioDto>> ObterTodosAsync();
        Task AtualizarAsync(Guid id, string nome, string email);
        Task AtualizarSenhaAsync(Guid id, string senhaAtual, string novaSenha);
        Task DeletarAsync(Guid id);
    }
} 