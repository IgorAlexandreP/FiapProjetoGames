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
        Task<UsuarioDetalhadoDto> ObterDetalhadoAsync(Guid id);
        Task<IEnumerable<UsuarioDto>> ObterTodosAsync();
        Task<IEnumerable<UsuarioDto>> ObterComFiltroAsync(UsuarioFiltroDto filtro);
        Task<IEnumerable<UsuarioResumoDto>> ObterResumoAsync(UsuarioFiltroDto filtro);
        Task AtualizarAsync(Guid id, string nome, string email);
        Task AtualizarSenhaAsync(Guid id, string senhaAtual, string novaSenha);
        Task AtivarAsync(Guid id);
        Task DesativarAsync(Guid id);
        Task BloquearAsync(Guid id, int minutos = 30);
        Task DesbloquearAsync(Guid id);
        Task TornarAdminAsync(Guid id);
        Task RemoverAdminAsync(Guid id);
        Task DeletarAsync(Guid id);
    }
} 