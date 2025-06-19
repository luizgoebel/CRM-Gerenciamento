using CRM.Domain.Entidades;
using CRM.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.DbContext;

public sealed class CrmDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options) { }
    public CrmDbContext() { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Additional model configurations can be added here
        modelBuilder.ApplyConfiguration(new ClienteEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PedidoItemEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PedidoEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProdutoEntityTypeConfiguration());
    }
}
