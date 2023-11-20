using Microsoft.EntityFrameworkCore;
using Gordo.Models;

namespace Gordo.Data
{
    public class AppDataContext : DbContext
    {
        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) { }

        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
