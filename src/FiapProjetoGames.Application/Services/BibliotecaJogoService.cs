using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiapProjetoGames.Domain.Entities;
using FiapProjetoGames.Domain.Repositories;
using FiapProjetoGames.Application.DTOs;

namespace FiapProjetoGames.Application.Services
{
    public class BibliotecaJogoService : IBibliotecaJogoService
    {
        private readonly IBibliotecaJogoRepository _bibliotecaJogoRepository;
        private readonly IJogoRepository _jogoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public BibliotecaJogoService(
            IBibliotecaJogoRepository bibliotecaJogoRepository,
            IJogoRepository jogoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _bibliotecaJogoRepository = bibliotecaJogoRepository;
            _jogoRepository = jogoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<BibliotecaJogoDto>> ObterBibliotecaAsync(Guid usuarioId)
        {
            var jogos = await _bibliotecaJogoRepository.ObterPorUsuarioIdAsync(usuarioId);
            return jogos.Select(j => new BibliotecaJogoDto
            {
                Id = j.Id,
                JogoId = j.JogoId,
                DataCompra = j.DataCompra,
                PrecoCompra = j.PrecoCompra,
                Jogo = new JogoDto
                {
                    Id = j.Jogo.Id,
                    Titulo = j.Jogo.Titulo,
                    Descricao = j.Jogo.Descricao,
                    Preco = j.Jogo.Preco
                }
            });
        }

        public async Task<BibliotecaJogoDto> AdicionarJogoAsync(Guid usuarioId, Guid jogoId)
        {
            var jogo = await _jogoRepository.ObterPorIdAsync(jogoId) ?? 
                throw new InvalidOperationException("Jogo não encontrado");

            var jaTemJogo = await _bibliotecaJogoRepository.UsuarioPossuiJogoAsync(usuarioId, jogoId);
            if (jaTemJogo)
            {
                throw new InvalidOperationException("Usuário já possui este jogo");
            }

            var bibliotecaJogo = new BibliotecaJogo(usuarioId, jogoId, jogo.Preco);
            await _bibliotecaJogoRepository.CriarAsync(bibliotecaJogo);

            return new BibliotecaJogoDto
            {
                Id = bibliotecaJogo.Id,
                UsuarioId = bibliotecaJogo.UsuarioId,
                JogoId = bibliotecaJogo.JogoId,
                DataCompra = bibliotecaJogo.DataCompra,
                PrecoCompra = bibliotecaJogo.PrecoCompra,
                Jogo = new JogoDto
                {
                    Id = jogo.Id,
                    Titulo = jogo.Titulo,
                    Descricao = jogo.Descricao,
                    Preco = jogo.Preco
                }
            };
        }

        public async Task RemoverJogoAsync(Guid usuarioId, Guid jogoId)
        {
            var bibliotecaJogo = await _bibliotecaJogoRepository.ObterPorUsuarioEJogoIdAsync(usuarioId, jogoId) ??
                throw new InvalidOperationException("Jogo não encontrado na biblioteca do usuário");

            await _bibliotecaJogoRepository.DeletarAsync(bibliotecaJogo.Id);
        }

        public async Task<BibliotecaJogoDto> ObterPorIdAsync(Guid id)
        {
            var bibliotecaJogo = await _bibliotecaJogoRepository.ObterPorIdAsync(id) ??
                throw new InvalidOperationException("Jogo não encontrado na biblioteca");

            return new BibliotecaJogoDto
            {
                Id = bibliotecaJogo.Id,
                JogoId = bibliotecaJogo.JogoId,
                DataCompra = bibliotecaJogo.DataCompra,
                PrecoCompra = bibliotecaJogo.PrecoCompra,
                Jogo = new JogoDto
                {
                    Id = bibliotecaJogo.Jogo.Id,
                    Titulo = bibliotecaJogo.Jogo.Titulo,
                    Descricao = bibliotecaJogo.Jogo.Descricao,
                    Preco = bibliotecaJogo.Jogo.Preco
                }
            };
        }

        public async Task<IEnumerable<BibliotecaJogoDto>> ObterPorUsuarioIdAsync(Guid usuarioId)
        {
            var jogos = await _bibliotecaJogoRepository.ObterPorUsuarioIdAsync(usuarioId);
            return jogos.Select(j => new BibliotecaJogoDto
            {
                Id = j.Id,
                JogoId = j.JogoId,
                DataCompra = j.DataCompra,
                PrecoCompra = j.PrecoCompra,
                Jogo = new JogoDto
                {
                    Id = j.Jogo.Id,
                    Titulo = j.Jogo.Titulo,
                    Descricao = j.Jogo.Descricao,
                    Preco = j.Jogo.Preco
                }
            });
        }

        public async Task<BibliotecaJogoDto> ComprarJogoAsync(Guid usuarioId, CompraJogoDto compraJogoDto)
        {
            var jogo = await _jogoRepository.ObterPorIdAsync(compraJogoDto.JogoId) ??
                throw new InvalidOperationException("Jogo não encontrado");

            var bibliotecaJogo = new BibliotecaJogo(usuarioId, compraJogoDto.JogoId, compraJogoDto.PrecoCompra);
            await _bibliotecaJogoRepository.CriarAsync(bibliotecaJogo);

            return new BibliotecaJogoDto
            {
                Id = bibliotecaJogo.Id,
                JogoId = bibliotecaJogo.JogoId,
                DataCompra = bibliotecaJogo.DataCompra,
                PrecoCompra = bibliotecaJogo.PrecoCompra,
                Jogo = new JogoDto
                {
                    Id = jogo.Id,
                    Titulo = jogo.Titulo,
                    Descricao = jogo.Descricao,
                    Preco = jogo.Preco
                }
            };
        }

        public async Task<bool> VerificarPropriedadeJogoAsync(Guid usuarioId, Guid jogoId)
        {
            var bibliotecaJogo = await _bibliotecaJogoRepository.ObterPorUsuarioEJogoIdAsync(usuarioId, jogoId);
            return bibliotecaJogo != null;
        }
    }
} 