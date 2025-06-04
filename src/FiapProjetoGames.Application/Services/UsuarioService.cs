using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
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
        private readonly string _jwtSecret;

        public UsuarioService(IUsuarioRepository usuarioRepository, string jwtSecret)
        {
            _usuarioRepository = usuarioRepository;
            _jwtSecret = jwtSecret;
        }

        public async Task<UsuarioDto> CadastrarAsync(CadastroUsuarioDto cadastroUsuarioDto)
        {
            var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(cadastroUsuarioDto.Email);
            if (usuarioExistente != null)
            {
                throw new InvalidOperationException("Email já está em uso");
            }

            var senhaHash = BC.HashPassword(cadastroUsuarioDto.Senha);
            var usuario = new Usuario(cadastroUsuarioDto.Nome, cadastroUsuarioDto.Email, senhaHash, cadastroUsuarioDto.IsAdmin);

            await _usuarioRepository.CriarAsync(usuario);

            var token = GerarToken(usuario);

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                IsAdmin = usuario.IsAdmin,
                Token = token
            };
        }

        public async Task<UsuarioDto> LoginAsync(LoginUsuarioDto loginUsuarioDto)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(loginUsuarioDto.Email);
            if (usuario == null || !BC.Verify(loginUsuarioDto.Senha, usuario.SenhaHash))
            {
                throw new InvalidOperationException("Email ou senha inválidos");
            }

            var token = GerarToken(usuario);

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                IsAdmin = usuario.IsAdmin,
                Token = token
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
                IsAdmin = usuario.IsAdmin
            };
        }

        public async Task<IEnumerable<UsuarioDto>> ObterTodosAsync()
        {
            var usuarios = await _usuarioRepository.ObterTodosAsync();
            var usuariosDto = new List<UsuarioDto>();

            foreach (var usuario in usuarios)
            {
                usuariosDto.Add(new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    IsAdmin = usuario.IsAdmin
                });
            }

            return usuariosDto;
        }

        public async Task AtualizarAsync(Guid id, string nome, string email)
        {
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
                    throw new InvalidOperationException("Email já está em uso");
                }
            }

            usuario.AtualizarDados(nome, email);
            await _usuarioRepository.AtualizarAsync(usuario);
        }

        public async Task AtualizarSenhaAsync(Guid id, string senhaAtual, string novaSenha)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado");
            }

            if (!BC.Verify(senhaAtual, usuario.SenhaHash))
            {
                throw new InvalidOperationException("Senha atual incorreta");
            }

            var novaSenhaHash = BC.HashPassword(novaSenha);
            usuario.AtualizarSenha(novaSenhaHash);
            await _usuarioRepository.AtualizarAsync(usuario);
        }

        public async Task DeletarAsync(Guid id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado");
            }

            await _usuarioRepository.DeletarAsync(id);
        }

        private string GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.IsAdmin ? "Admin" : "User")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
} 