using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FiapProjetoGames.Application.DTOs;
using FiapProjetoGames.Application.Services;
using System.Security.Claims;

namespace FiapProjetoGames.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/biblioteca")]
    public class BibliotecaJogosController : ControllerBase
    {
        private readonly IBibliotecaJogoService _bibliotecaJogoService;

        public BibliotecaJogosController(IBibliotecaJogoService bibliotecaJogoService)
        {
            _bibliotecaJogoService = bibliotecaJogoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BibliotecaJogoDto>>> ObterBiblioteca()
        {
            var usuarioId = ObterUsuarioId();
            var jogos = await _bibliotecaJogoService.ObterBibliotecaAsync(usuarioId);
            return Ok(jogos);
        }

        [HttpPost("{jogoId}")]
        public async Task<ActionResult<BibliotecaJogoDto>> ComprarJogo(string jogoId, [FromQuery] Guid? usuarioAlvoId = null)
        {
            if (string.IsNullOrEmpty(jogoId))
            {
                return BadRequest("ID do jogo não fornecido");
            }

            var usuarioLogadoId = ObterUsuarioId();
            var usuarioFinalId = usuarioAlvoId ?? usuarioLogadoId;

            // Se o usuário não é admin e está tentando adicionar para outro usuário
            if (usuarioAlvoId.HasValue && usuarioAlvoId.Value != usuarioLogadoId && !User.IsInRole("Admin"))
            {
                return Forbid("Apenas administradores podem adicionar jogos para outros usuários");
            }

            var jogo = await _bibliotecaJogoService.AdicionarJogoAsync(usuarioFinalId, Guid.Parse(jogoId));
            return CreatedAtAction(nameof(ObterBiblioteca), new { }, jogo);
        }

        [HttpDelete("{jogoId}")]
        public async Task<ActionResult> RemoverJogo(string jogoId, [FromQuery] Guid? usuarioAlvoId = null)
        {
            if (string.IsNullOrEmpty(jogoId))
            {
                return BadRequest("ID do jogo não fornecido");
            }

            var usuarioLogadoId = ObterUsuarioId();
            var usuarioFinalId = usuarioAlvoId ?? usuarioLogadoId;

            // Se o usuário não é admin e está tentando remover de outro usuário
            if (usuarioAlvoId.HasValue && usuarioAlvoId.Value != usuarioLogadoId && !User.IsInRole("Admin"))
            {
                return Forbid("Apenas administradores podem remover jogos de outros usuários");
            }

            await _bibliotecaJogoService.RemoverJogoAsync(usuarioFinalId, Guid.Parse(jogoId));
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BibliotecaJogoDto>> ObterPorId(Guid id)
        {
            var bibliotecaJogo = await _bibliotecaJogoService.ObterPorIdAsync(id);
            return Ok(bibliotecaJogo);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<BibliotecaJogoDto>>> ObterPorUsuarioId(Guid usuarioId)
        {
            var bibliotecaJogos = await _bibliotecaJogoService.ObterPorUsuarioIdAsync(usuarioId);
            return Ok(bibliotecaJogos);
        }

        [HttpGet("verificar/{jogoId}")]
        public async Task<ActionResult<bool>> VerificarPropriedadeJogo(Guid jogoId, [FromQuery] Guid? usuarioAlvoId = null)
        {
            var usuarioLogadoId = ObterUsuarioId();
            var usuarioFinalId = usuarioAlvoId ?? usuarioLogadoId;

            // Se o usuário não é admin e está tentando verificar outro usuário
            if (usuarioAlvoId.HasValue && usuarioAlvoId.Value != usuarioLogadoId && !User.IsInRole("Admin"))
            {
                return Forbid("Apenas administradores podem verificar jogos de outros usuários");
            }

            var possuiJogo = await _bibliotecaJogoService.VerificarPropriedadeJogoAsync(usuarioFinalId, jogoId);
            return Ok(possuiJogo);
        }

        private Guid ObterUsuarioId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? 
                throw new UnauthorizedAccessException("Usuário não autenticado");
            return Guid.Parse(claim.Value);
        }
    }
} 