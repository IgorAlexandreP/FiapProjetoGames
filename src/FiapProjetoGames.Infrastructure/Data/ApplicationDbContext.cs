using Microsoft.EntityFrameworkCore;
using FiapProjetoGames.Domain.Entities;

namespace FiapProjetoGames.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<BibliotecaJogo> BibliotecaJogos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.SenhaHash).IsRequired();
                entity.Property(e => e.IsAdmin).HasDefaultValue(false);
                entity.Property(e => e.Ativo).HasDefaultValue(true);
                entity.Property(e => e.DataCriacao).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.TentativasLogin).HasDefaultValue(0);
            });

            modelBuilder.Entity<Jogo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).IsRequired();
                entity.Property(e => e.Descricao).IsRequired();
                entity.Property(e => e.Preco).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<BibliotecaJogo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Jogo)
                    .WithMany()
                    .HasForeignKey(e => e.JogoId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.PrecoCompra).HasColumnType("decimal(18,2)");
            });
        }
    }
} 