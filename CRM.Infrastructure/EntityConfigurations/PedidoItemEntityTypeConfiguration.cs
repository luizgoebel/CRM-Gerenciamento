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

        builder.Property(pi => pi.ProdutoId).IsRequired();
        builder.Property(pi => pi.Quantidade).IsRequired();
        builder.Property(pi => pi.Subtotal).IsRequired();

        builder.HasOne<Pedido>()
               .WithMany(p => p.Itens)
               .HasForeignKey(pi => pi.PedidoId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
