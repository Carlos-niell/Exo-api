using Exo.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Exo.WebApi.Contexts
{
    public class ExoContext : DbContext
    {
        public ExoContext()
        {
        }

        public ExoContext(DbContextOptions<ExoContext> options) : base(options)
        {
        }

        public DbSet<Projeto> Projetos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=ExoApi;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Projeto>(entity =>
            {
                entity.ToTable("Projetos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.NomeDoProjeto).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Area).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
