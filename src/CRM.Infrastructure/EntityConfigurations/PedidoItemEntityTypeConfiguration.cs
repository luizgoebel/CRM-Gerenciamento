using CRM.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.EntityConfigurations;

public class PedidoItemEntityTypeConfiguration : IEntityTypeConfiguration<PedidoItem>
{
    public void Configure(EntityTypeBuilder<PedidoItem> builder)
    {
        builder.ToTable("pedido_item");
        builder.HasKey(pi => pi.Id);

        builder.Property(p => p.PrecoUnitario)
        .HasColumnType("decimal(10,2)");

        builder.Property(p => p.Subtotal)
               .HasColumnType("decimal(12,2)");


        builder.HasOne(pi => pi.Pedido)
               .WithMany(p => p.Itens)
               .HasForeignKey(pi => pi.PedidoId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pi => pi.Produto)
               .WithMany()
               .HasForeignKey(pi => pi.ProdutoId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

