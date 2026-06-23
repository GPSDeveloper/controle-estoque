using ControleEstoque.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Setor> Setores => Set<Setor>();
    public DbSet<Material> Materiais => Set<Material>();
    public DbSet<Saida> Saidas => Set<Saida>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Login).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Login).IsUnique();
            entity.Property(e => e.SenhaHash).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Setor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.Nome).IsUnique();
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Quantidade).HasPrecision(18, 2);
            entity.Property(e => e.PrecoUnitario).HasPrecision(18, 2);
            entity.Property(e => e.Localizacao).HasMaxLength(300);
        });

        modelBuilder.Entity<Saida>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantidade).HasPrecision(18, 2);
            entity.Property(e => e.PrecoUnitarioNaSaida).HasPrecision(18, 2);
            entity.Property(e => e.NomeRetirante).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Identificacao).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TipoIdentificacao).HasConversion<int>();

            entity.HasOne(e => e.Material)
                .WithMany(m => m.Saidas)
                .HasForeignKey(e => e.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Setor)
                .WithMany(s => s.Saidas)
                .HasForeignKey(e => e.SetorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
