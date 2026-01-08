using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FiapProjetoGames.Domain.Entities;
using FiapProjetoGames.Domain.Repositories;
using FiapProjetoGames.Infrastructure.Data;

namespace FiapProjetoGames.Infrastructure.Repositories
{
    public class JogoRepository : IJogoRepository
    {
        private readonly ApplicationDbContext _context;

        public JogoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Jogo> ObterPorIdAsync(Guid id)
        {
            return await _context.Jogos.FindAsync(id);
        }

        public async Task<IEnumerable<Jogo>> ObterTodosAsync()
        {
            return await _context.Jogos.ToListAsync();
        }

        public async Task<Jogo> CriarAsync(Jogo jogo)
        {
            _context.Jogos.Add(jogo);
            await _context.SaveChangesAsync();
            return jogo;
        }

        public async Task AtualizarAsync(Jogo jogo)
        {
            _context.Entry(jogo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeletarAsync(Guid id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo != null)
            {
                _context.Jogos.Remove(jogo);
                await _context.SaveChangesAsync();
            }
        }
    }
} 