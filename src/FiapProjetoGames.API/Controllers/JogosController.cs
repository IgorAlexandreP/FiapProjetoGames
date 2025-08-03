using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FiapProjetoGames.Application.DTOs;
using FiapProjetoGames.Application.Services;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace FiapProjetoGames.API.Controllers
{
    [ApiController]
    [Route("api/jogos")]
    public class JogosController : ControllerBase
    {
        private readonly IJogoService _jogoService;
        private readonly ILogger<JogosController> _logger;

        public JogosController(IJogoService jogoService, ILogger<JogosController> logger)
        {
            _jogoService = jogoService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JogoDto>> ObterPorId(Guid id)
        {
            var jogo = await _jogoService.ObterPorIdAsync(id);
            return Ok(jogo);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoDto>>> ObterTodos()
        {
            var jogos = await _jogoService.ObterTodosAsync();
            return Ok(jogos);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<JogoDto>> Criar(CadastroJogoDto cadastroJogoDto)
        {
            _logger.LogInformation("Tentando criar jogo. Usuário autenticado: {IsAuthenticated}", User.Identity?.IsAuthenticated);
            _logger.LogInformation("Claims do usuário:");
            foreach (var claim in User.Claims)
            {
                _logger.LogInformation("  {Type}: {Value}", claim.Type, claim.Value);
            }
            
            if (!User.IsInRole("Admin"))
            {
                _logger.LogWarning("Usuário não tem role Admin");
                return Forbid();
            }

            var jogo = await _jogoService.CriarAsync(cadastroJogoDto);
            return CreatedAtAction(nameof(ObterPorId), new { id = jogo.Id }, jogo);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Atualizar(Guid id, AtualizacaoJogoDto atualizacaoJogoDto)
        {
            await _jogoService.AtualizarAsync(id, atualizacaoJogoDto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Deletar(Guid id)
        {
            await _jogoService.DeletarAsync(id);
            return NoContent();
        }
    }
} 