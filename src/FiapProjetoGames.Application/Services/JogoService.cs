using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FiapProjetoGames.Application.DTOs;
using FiapProjetoGames.Domain.Entities;
using FiapProjetoGames.Domain.Repositories;

namespace FiapProjetoGames.Application.Services
{
    public class JogoService : IJogoService
    {
        private readonly IJogoRepository _jogoRepository;

        public JogoService(IJogoRepository jogoRepository)
        {
            _jogoRepository = jogoRepository;
        }

        public async Task<JogoDto> ObterPorIdAsync(Guid id)
        {
            var jogo = await _jogoRepository.ObterPorIdAsync(id);
            if (jogo == null)
            {
                throw new InvalidOperationException("Jogo não encontrado");
            }

            return new JogoDto
            {
                Id = jogo.Id,
                Titulo = jogo.Titulo,
                Descricao = jogo.Descricao,
                Preco = jogo.Preco
            };
        }

        public async Task<IEnumerable<JogoDto>> ObterTodosAsync()
        {
            var jogos = await _jogoRepository.ObterTodosAsync();
            var jogosDto = new List<JogoDto>();

            foreach (var jogo in jogos)
            {
                jogosDto.Add(new JogoDto
                {
                    Id = jogo.Id,
                    Titulo = jogo.Titulo,
                    Descricao = jogo.Descricao,
                    Preco = jogo.Preco
                });
            }

            return jogosDto;
        }

        public async Task<JogoDto> CriarAsync(CadastroJogoDto cadastroJogoDto)
        {
            var jogo = new Jogo(
                cadastroJogoDto.Titulo,
                cadastroJogoDto.Descricao,
                cadastroJogoDto.Preco
            );

            await _jogoRepository.CriarAsync(jogo);

            return new JogoDto
            {
                Id = jogo.Id,
                Titulo = jogo.Titulo,
                Descricao = jogo.Descricao,
                Preco = jogo.Preco
            };
        }

        public async Task AtualizarAsync(Guid id, AtualizacaoJogoDto atualizacaoJogoDto)
        {
            var jogo = await _jogoRepository.ObterPorIdAsync(id);
            if (jogo == null)
            {
                throw new InvalidOperationException("Jogo não encontrado");
            }

            jogo.Atualizar(
                atualizacaoJogoDto.Titulo,
                atualizacaoJogoDto.Descricao,
                atualizacaoJogoDto.Preco
            );

            await _jogoRepository.AtualizarAsync(jogo);
        }

        public async Task DeletarAsync(Guid id)
        {
            var jogo = await _jogoRepository.ObterPorIdAsync(id);
            if (jogo == null)
            {
                throw new InvalidOperationException("Jogo não encontrado");
            }

            await _jogoRepository.DeletarAsync(id);
        }
    }
} 