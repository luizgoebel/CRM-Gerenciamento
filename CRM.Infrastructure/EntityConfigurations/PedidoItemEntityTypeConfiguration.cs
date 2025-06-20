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

        builder.HasOne<Pedido>()
               .WithMany(p => p.Itens)
               .HasForeignKey(pi => pi.PedidoId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Produto>()
               .WithMany()
               .HasForeignKey(p => p.ProdutoId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
