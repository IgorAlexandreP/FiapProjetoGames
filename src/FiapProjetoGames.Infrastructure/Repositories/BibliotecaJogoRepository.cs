using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FiapProjetoGames.Domain.Entities;
using FiapProjetoGames.Domain.Repositories;
using FiapProjetoGames.Infrastructure.Data;

namespace FiapProjetoGames.Infrastructure.Repositories
{
    public class BibliotecaJogoRepository : IBibliotecaJogoRepository
    {
        private readonly ApplicationDbContext _context;

        public BibliotecaJogoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BibliotecaJogo> ObterPorIdAsync(Guid id)
        {
            return await _context.BibliotecaJogos
                .Include(bg => bg.Jogo)
                .FirstOrDefaultAsync(bg => bg.Id == id);
        }

        public async Task<IEnumerable<BibliotecaJogo>> ObterPorUsuarioIdAsync(Guid usuarioId)
        {
            return await _context.BibliotecaJogos
                .Include(bg => bg.Jogo)
                .Where(bg => bg.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<BibliotecaJogo> CriarAsync(BibliotecaJogo bibliotecaJogo)
        {
            _context.BibliotecaJogos.Add(bibliotecaJogo);
            await _context.SaveChangesAsync();
            return bibliotecaJogo;
        }

        public async Task<bool> UsuarioPossuiJogoAsync(Guid usuarioId, Guid jogoId)
        {
            return await _context.BibliotecaJogos
                .AnyAsync(bg => bg.UsuarioId == usuarioId && bg.JogoId == jogoId);
        }

        public async Task DeletarAsync(Guid id)
        {
            var bibliotecaJogo = await _context.BibliotecaJogos.FindAsync(id);
            if (bibliotecaJogo != null)
            {
                _context.BibliotecaJogos.Remove(bibliotecaJogo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<BibliotecaJogo> ObterPorUsuarioEJogoIdAsync(Guid usuarioId, Guid jogoId)
        {
            return await _context.BibliotecaJogos
                .Include(bg => bg.Jogo)
                .FirstOrDefaultAsync(bg => bg.UsuarioId == usuarioId && bg.JogoId == jogoId);
        }
    }
} 