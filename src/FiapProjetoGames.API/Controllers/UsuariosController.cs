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
        private readonly ILogService _logService;
        private readonly IMetricsService _metricsService;

        public UsuariosController(IUsuarioService usuarioService, ILogService logService, IMetricsService metricsService)
        {
            _usuarioService = usuarioService;
            _logService = logService;
            _metricsService = metricsService;
        }

        [HttpPost("cadastro")]
        public async Task<ActionResult<ApiResponseDto<UsuarioDto>>> Cadastrar(CadastroUsuarioDto cadastroUsuarioDto)
        {
            var startTime = DateTime.UtcNow;
            try
            {
                var usuario = await _usuarioService.CadastrarAsync(cadastroUsuarioDto);
                await _logService.LogInfoAsync("Usuário cadastrado com sucesso", new { Email = usuario.Email, Id = usuario.Id });
                
                // Registrar métricas de sucesso
                await _metricsService.IncrementCounterAsync("user_registration", "success");
                await _metricsService.RecordTimingAsync("user_registration_time", DateTime.UtcNow - startTime);
                
                return Ok(new ApiResponseDto<UsuarioDto>
                {
                    Success = true,
                    Message = "Usuário cadastrado com sucesso!",
                    Data = usuario
                });
            }
            catch (InvalidOperationException ex)
            {
                await _logService.LogErrorAsync("Erro ao cadastrar usuário", ex, cadastroUsuarioDto);
                await _metricsService.IncrementCounterAsync("user_registration", "error");
                throw; // Deixar o middleware tratar
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Erro inesperado ao cadastrar usuário", ex, cadastroUsuarioDto);
                await _metricsService.IncrementCounterAsync("user_registration", "error");
                throw; // Deixar o middleware tratar
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<UsuarioDto>>> Login(LoginUsuarioDto loginUsuarioDto)
        {
            var startTime = DateTime.UtcNow;
            try
            {
                var usuario = await _usuarioService.LoginAsync(loginUsuarioDto);
                await _logService.LogSecurityAsync("Login realizado com sucesso", new { Email = usuario.Email, Id = usuario.Id });
                
                // Registrar métricas de sucesso
                await _metricsService.IncrementCounterAsync("user_login", "success");
                await _metricsService.RecordTimingAsync("user_login_time", DateTime.UtcNow - startTime);
                
                return Ok(new ApiResponseDto<UsuarioDto>
                {
                    Success = true,
                    Message = "Login realizado com sucesso!",
                    Data = usuario
                });
            }
            catch (InvalidOperationException ex)
            {
                await _logService.LogSecurityAsync("Tentativa de login falhou", new { Email = loginUsuarioDto.Email, Error = ex.Message });
                await _metricsService.IncrementCounterAsync("user_login", "error");
                throw; // Deixar o middleware tratar
            }
            catch (Exception ex)
            {
                await _logService.LogSecurityAsync("Erro inesperado no login", new { Email = loginUsuarioDto.Email, Error = ex.Message });
                await _metricsService.IncrementCounterAsync("user_login", "error");
                throw; // Deixar o middleware tratar
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> ObterPorId(Guid id)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);
            return Ok(usuario);
        }

        [Authorize]
        [HttpGet("{id}/detalhado")]
        public async Task<ActionResult<UsuarioDetalhadoDto>> ObterDetalhado(Guid id)
        {
            var usuario = await _usuarioService.ObterDetalhadoAsync(id);
            return Ok(usuario);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> ObterTodos([FromQuery] UsuarioFiltroDto filtro)
        {
            var usuarios = await _usuarioService.ObterComFiltroAsync(filtro);
            return Ok(usuarios);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("resumo")]
        public async Task<ActionResult<IEnumerable<UsuarioResumoDto>>> ObterResumo([FromQuery] UsuarioFiltroDto filtro)
        {
            var usuarios = await _usuarioService.ObterResumoAsync(filtro);
            return Ok(usuarios);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Atualizar(Guid id, AtualizacaoUsuarioDto atualizacaoUsuarioDto)
        {
            try
            {
                await _usuarioService.AtualizarAsync(id, atualizacaoUsuarioDto.Nome, atualizacaoUsuarioDto.Email);
                await _logService.LogAuditAsync("Atualização de dados", id.ToString(), atualizacaoUsuarioDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Erro ao atualizar usuário", ex, new { Id = id, Dados = atualizacaoUsuarioDto });
                throw;
            }
        }

        [Authorize]
        [HttpPut("{id}/senha")]
        public async Task<ActionResult> AtualizarSenha(Guid id, AtualizacaoSenhaDto atualizacaoSenhaDto)
        {
            try
            {
                await _usuarioService.AtualizarSenhaAsync(id, atualizacaoSenhaDto.SenhaAtual, atualizacaoSenhaDto.NovaSenha);
                await _logService.LogSecurityAsync("Senha alterada", new { Id = id });
                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogSecurityAsync("Tentativa de alteração de senha falhou", new { Id = id, Error = ex.Message });
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/ativar")]
        public async Task<ActionResult> Ativar(Guid id)
        {
            try
            {
                await _usuarioService.AtivarAsync(id);
                await _logService.LogAuditAsync("Usuário ativado", id.ToString());
                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Erro ao ativar usuário", ex, new { Id = id });
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/desativar")]
        public async Task<ActionResult> Desativar(Guid id)
        {
            try
            {
                await _usuarioService.DesativarAsync(id);
                await _logService.LogAuditAsync("Usuário desativado", id.ToString());
                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Erro ao desativar usuário", ex, new { Id = id });
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/bloquear")]
        public async Task<ActionResult> Bloquear(Guid id, [FromQuery] int minutos = 30)
        {
            try
            {
                await _usuarioService.BloquearAsync(id, minutos);
                await _logService.LogAuditAsync("Usuário bloqueado", id.ToString(), new { minutos });
                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Erro ao bloquear usuário", ex, new { Id = id, minutos });
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/desbloquear")]
        public async Task<ActionResult> Desbloquear(Guid id)
        {
            try
            {
                await _usuarioService.DesbloquearAsync(id);
                await _logService.LogAuditAsync("Usuário desbloqueado", id.ToString());
                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Erro ao desbloquear usuário", ex, new { Id = id });
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/tornar-admin")]
        public async Task<ActionResult> TornarAdmin(Guid id)
        {
            try
            {
                await _usuarioService.TornarAdminAsync(id);
                await _logService.LogAuditAsync("Usuário promovido a admin", id.ToString());
                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Erro ao promover usuário a admin", ex, new { Id = id });
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/remover-admin")]
        public async Task<ActionResult> RemoverAdmin(Guid id)
        {
            try
            {
                await _usuarioService.RemoverAdminAsync(id);
                await _logService.LogAuditAsync("Privilégios de admin removidos", id.ToString());
                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Erro ao remover privilégios de admin", ex, new { Id = id });
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Deletar(Guid id)
        {
            try
            {
                await _usuarioService.DeletarAsync(id);
                await _logService.LogAuditAsync("Usuário deletado", id.ToString());
                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Erro ao deletar usuário", ex, new { Id = id });
                throw;
            }
        }
    }
} 