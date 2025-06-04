using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FiapProjetoGames.Application.DTOs;
using FiapProjetoGames.Application.Services;

namespace FiapProjetoGames.API.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("cadastro")]
        public async Task<ActionResult<UsuarioDto>> Cadastrar(CadastroUsuarioDto cadastroUsuarioDto)
        {
            var usuario = await _usuarioService.CadastrarAsync(cadastroUsuarioDto);
            return Ok(usuario);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDto>> Login(LoginUsuarioDto loginUsuarioDto)
        {
            var usuario = await _usuarioService.LoginAsync(loginUsuarioDto);
            return Ok(usuario);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> ObterPorId(Guid id)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);
            return Ok(usuario);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> ObterTodos()
        {
            var usuarios = await _usuarioService.ObterTodosAsync();
            return Ok(usuarios);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Atualizar(Guid id, AtualizacaoUsuarioDto atualizacaoUsuarioDto)
        {
            await _usuarioService.AtualizarAsync(id, atualizacaoUsuarioDto.Nome, atualizacaoUsuarioDto.Email);
            return NoContent();
        }

        [Authorize]
        [HttpPut("{id}/senha")]
        public async Task<ActionResult> AtualizarSenha(Guid id, AtualizacaoSenhaDto atualizacaoSenhaDto)
        {
            await _usuarioService.AtualizarSenhaAsync(id, atualizacaoSenhaDto.SenhaAtual, atualizacaoSenhaDto.NovaSenha);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Deletar(Guid id)
        {
            await _usuarioService.DeletarAsync(id);
            return NoContent();
        }
    }
} 