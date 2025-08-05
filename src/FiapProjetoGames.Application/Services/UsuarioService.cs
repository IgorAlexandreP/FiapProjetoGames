using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using FiapProjetoGames.Domain.Entities;
using FiapProjetoGames.Domain.Repositories;
using FiapProjetoGames.Application.DTOs;
using BC = BCrypt.Net.BCrypt;

namespace FiapProjetoGames.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogService _logService;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpirationHours;

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogService logService, string jwtSecret, string jwtIssuer = "FiapProjetoGames", string jwtAudience = "FiapProjetoGamesUsers", int jwtExpirationHours = 24)
        {
            _usuarioRepository = usuarioRepository;
            _logService = logService;
            _jwtSecret = jwtSecret;
            _jwtIssuer = jwtIssuer;
            _jwtAudience = jwtAudience;
            _jwtExpirationHours = jwtExpirationHours;
        }

        public async Task<UsuarioDto> CadastrarAsync(CadastroUsuarioDto cadastroUsuarioDto)
        {
            await _logService.LogInfoAsync("Tentativa de cadastro de usuário", new { email = cadastroUsuarioDto.Email });

            var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(cadastroUsuarioDto.Email);
            if (usuarioExistente != null)
            {
                await _logService.LogWarningAsync("Tentativa de cadastro com email já existente", new { email = cadastroUsuarioDto.Email });
                throw new InvalidOperationException("Email já está em uso");
            }

            var senhaHash = BC.HashPassword(cadastroUsuarioDto.Senha);
            var usuario = new Usuario(cadastroUsuarioDto.Nome, cadastroUsuarioDto.Email, senhaHash, cadastroUsuarioDto.IsAdmin);

            await _usuarioRepository.CriarAsync(usuario);

            var token = GerarToken(usuario);

            await _logService.LogAuditAsync("UsuarioCadastrado", usuario.Id.ToString(), new { email = usuario.Email, isAdmin = usuario.IsAdmin });

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                IsAdmin = usuario.IsAdmin,
                Token = token,
                DataCriacao = usuario.DataCriacao,
                DataAtualizacao = usuario.DataAtualizacao,
                Ativo = usuario.Ativo
            };
        }

        public async Task<UsuarioDto> LoginAsync(LoginUsuarioDto loginUsuarioDto)
        {
            await _logService.LogInfoAsync("Tentativa de login", new { email = loginUsuarioDto.Email });

            var usuario = await _usuarioRepository.ObterPorEmailAsync(loginUsuarioDto.Email);
            
            if (usuario == null)
            {
                await _logService.LogSecurityAsync("Tentativa de login com email inexistente", new { email = loginUsuarioDto.Email });
                throw new InvalidOperationException("Email ou senha inválidos");
            }

            if (!usuario.Ativo)
            {
                await _logService.LogSecurityAsync("Tentativa de login em conta inativa", new { email = loginUsuarioDto.Email });
                throw new InvalidOperationException("Conta inativa");
            }

            if (usuario.EstaBloqueado())
            {
                await _logService.LogSecurityAsync("Tentativa de login em conta bloqueada", new { email = loginUsuarioDto.Email });
                throw new InvalidOperationException("Conta temporariamente bloqueada");
            }

            if (!BC.Verify(loginUsuarioDto.Senha, usuario.SenhaHash))
            {
                usuario.IncrementarTentativaLogin();
                await _usuarioRepository.AtualizarAsync(usuario);
                
                await _logService.LogSecurityAsync("Tentativa de login com senha incorreta", new { email = loginUsuarioDto.Email });
                throw new InvalidOperationException("Email ou senha inválidos");
            }

            // Reset das tentativas de login em caso de sucesso
            usuario.LoginBemSucedido();
            await _usuarioRepository.AtualizarAsync(usuario);

            var token = GerarToken(usuario);

            await _logService.LogAuditAsync("UsuarioLogado", usuario.Id.ToString(), new { email = usuario.Email });

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                IsAdmin = usuario.IsAdmin,
                Token = token,
                DataCriacao = usuario.DataCriacao,
                DataAtualizacao = usuario.DataAtualizacao,
                Ativo = usuario.Ativo
            };
        }

        public async Task<UsuarioDto> ObterPorIdAsync(Guid id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado");
            }

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                IsAdmin = usuario.IsAdmin,
                DataCriacao = usuario.DataCriacao,
                DataAtualizacao = usuario.DataAtualizacao,
                Ativo = usuario.Ativo
            };
        }

        public async Task<UsuarioDetalhadoDto> ObterDetalhadoAsync(Guid id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado");
            }

            return new UsuarioDetalhadoDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                IsAdmin = usuario.IsAdmin,
                DataCriacao = usuario.DataCriacao,
                DataAtualizacao = usuario.DataAtualizacao,
                UltimoLogin = usuario.UltimoLogin,
                TentativasLogin = usuario.TentativasLogin,
                BloqueadoAte = usuario.BloqueadoAte,
                Ativo = usuario.Ativo
            };
        }

        public async Task<IEnumerable<UsuarioDto>> ObterTodosAsync()
        {
            var usuarios = await _usuarioRepository.ObterTodosAsync();
            return usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                IsAdmin = u.IsAdmin,
                DataCriacao = u.DataCriacao,
                DataAtualizacao = u.DataAtualizacao,
                Ativo = u.Ativo
            });
        }

        public async Task<IEnumerable<UsuarioDto>> ObterComFiltroAsync(UsuarioFiltroDto filtro)
        {
            var usuarios = await _usuarioRepository.ObterTodosAsync();
            
            var query = usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(filtro.Nome))
                query = query.Where(u => u.Nome.Contains(filtro.Nome, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filtro.Email))
                query = query.Where(u => u.Email.Contains(filtro.Email, StringComparison.OrdinalIgnoreCase));

            if (filtro.IsAdmin.HasValue)
                query = query.Where(u => u.IsAdmin == filtro.IsAdmin.Value);

            if (filtro.Ativo.HasValue)
                query = query.Where(u => u.Ativo == filtro.Ativo.Value);

            if (filtro.DataCriacaoInicio.HasValue)
                query = query.Where(u => u.DataCriacao >= filtro.DataCriacaoInicio.Value);

            if (filtro.DataCriacaoFim.HasValue)
                query = query.Where(u => u.DataCriacao <= filtro.DataCriacaoFim.Value);

            return query.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                IsAdmin = u.IsAdmin,
                DataCriacao = u.DataCriacao,
                DataAtualizacao = u.DataAtualizacao,
                Ativo = u.Ativo
            });
        }

        public async Task<IEnumerable<UsuarioResumoDto>> ObterResumoAsync(UsuarioFiltroDto filtro)
        {
            var usuarios = await ObterComFiltroAsync(filtro);
            return usuarios.Select(u => new UsuarioResumoDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                IsAdmin = u.IsAdmin,
                Ativo = u.Ativo
            });
        }

        public async Task AtualizarAsync(Guid id, string nome, string email)
        {
            await _logService.LogInfoAsync("Tentativa de atualização de usuário", new { id, nome, email });

            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado");
            }

            if (!string.IsNullOrEmpty(email) && email != usuario.Email)
            {
                var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(email);
                if (usuarioExistente != null)
                {
                    await _logService.LogWarningAsync("Tentativa de atualização com email já existente", new { id, email });
                    throw new InvalidOperationException("Email já está em uso");
                }
            }

            usuario.AtualizarDados(nome, email);
            await _usuarioRepository.AtualizarAsync(usuario);

            await _logService.LogAuditAsync("UsuarioAtualizado", id.ToString(), new { nome, email });
        }

        public async Task AtualizarSenhaAsync(Guid id, string senhaAtual, string novaSenha)
        {
            await _logService.LogInfoAsync("Tentativa de atualização de senha", new { id });

            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado");
            }

            if (!BC.Verify(senhaAtual, usuario.SenhaHash))
            {
                await _logService.LogSecurityAsync("Tentativa de atualização de senha com senha atual incorreta", new { id });
                throw new InvalidOperationException("Senha atual incorreta");
            }

            var novaSenhaHash = BC.HashPassword(novaSenha);
            usuario.AtualizarSenha(novaSenhaHash);
            await _usuarioRepository.AtualizarAsync(usuario);

            await _logService.LogAuditAsync("SenhaAtualizada", id.ToString());
        }

        public async Task AtivarAsync(Guid id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado");
            }

            usuario.Ativar();
            await _usuarioRepository.AtualizarAsync(usuario);

            await _logService.LogAuditAsync("UsuarioAtivado", id.ToString());
        }

        public async Task DesativarAsync(Guid id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado");
            }

            usuario.Desativar();
            await _usuarioRepository.AtualizarAsync(usuario);

            await _logService.LogAuditAsync("UsuarioDesativado", id.ToString());
        }

        public async Task BloquearAsync(Guid id, int minutos = 30)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado");
            }

            usuario.Bloquear(minutos);
            await _usuarioRepository.AtualizarAsync(usuario);

            await _logService.LogAuditAsync("UsuarioBloqueado", id.ToString(), new { minutos });
        }

        public async Task DesbloquearAsync(Guid id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado");
            }

            usuario.Desbloquear();
            await _usuarioRepository.AtualizarAsync(usuario);

            await _logService.LogAuditAsync("UsuarioDesbloqueado", id.ToString());
        }

        public async Task TornarAdminAsync(Guid id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado");
            }

            usuario.TornarAdmin();
            await _usuarioRepository.AtualizarAsync(usuario);

            await _logService.LogAuditAsync("UsuarioTornadoAdmin", id.ToString());
        }

        public async Task RemoverAdminAsync(Guid id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado");
            }

            usuario.RemoverAdmin();
            await _usuarioRepository.AtualizarAsync(usuario);

            await _logService.LogAuditAsync("AdminRemovido", id.ToString());
        }

        public async Task DeletarAsync(Guid id)
        {
            await _logService.LogInfoAsync("Tentativa de exclusão de usuário", new { id });

            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado");
            }

            await _usuarioRepository.DeletarAsync(id);

            await _logService.LogAuditAsync("UsuarioDeletado", id.ToString());
        }

        private string GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            // Criar uma chave de 32 bytes (256 bits) usando SHA256 da chave secreta
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(_jwtSecret));
                var key = new SymmetricSecurityKey(keyBytes);
                
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new Claim(ClaimTypes.Email, usuario.Email),
                        new Claim(ClaimTypes.Role, usuario.IsAdmin ? "Admin" : "User"),
                        new Claim("name", usuario.Nome),
                        new Claim("email", usuario.Email)
                    }),
                    Issuer = _jwtIssuer,
                    Audience = _jwtAudience,
                    Expires = DateTime.UtcNow.AddHours(_jwtExpirationHours),
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
        }
    }
} 