using CRM.Domain.Entidades;
using CRM.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

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

    public override int SaveChanges()
    {
        AtualizarDatas();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AtualizarDatas();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AtualizarDatas()
    {
        DateTime dataAtual = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);

        foreach (var entry in ChangeTracker.Entries<IBaseModel>())
        {
            IBaseModel baseModel = entry.Entity;

            if (entry.State == EntityState.Added && baseModel.DataCriacao == null)
                baseModel.DataCriacao = dataAtual;

            baseModel.DataModificacao = dataAtual;
        }
    }
}