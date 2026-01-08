using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FiapProjetoGames.Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseMySql("Server=localhost;Port=3306;Database=FiapProjetoGames;User Id=root;Password=;", 
                ServerVersion.AutoDetect("Server=localhost;Port=3306;Database=FiapProjetoGames;User Id=root;Password=;"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
} 