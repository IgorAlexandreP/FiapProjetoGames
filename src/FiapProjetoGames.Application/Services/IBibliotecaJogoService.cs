using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiapProjetoGames.Application.DTOs;

namespace FiapProjetoGames.Application.Services
{
    public interface IBibliotecaJogoService
    {
        Task<IEnumerable<BibliotecaJogoDto>> ObterBibliotecaAsync(Guid usuarioId);
        Task<BibliotecaJogoDto> AdicionarJogoAsync(Guid usuarioId, Guid jogoId);
        Task RemoverJogoAsync(Guid usuarioId, Guid jogoId);
        Task<BibliotecaJogoDto> ObterPorIdAsync(Guid id);
        Task<IEnumerable<BibliotecaJogoDto>> ObterPorUsuarioIdAsync(Guid usuarioId);
        Task<BibliotecaJogoDto> ComprarJogoAsync(Guid usuarioId, CompraJogoDto compraJogoDto);
        Task<bool> VerificarPropriedadeJogoAsync(Guid usuarioId, Guid jogoId);
    }
} 