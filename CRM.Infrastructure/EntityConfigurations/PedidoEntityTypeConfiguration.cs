using CRM.Domain.Entidades;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.EntityConfigurations
{
    public class PedidoEntityTypeConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("pedido");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Cliente).IsRequired();
            builder.Property(p => p.DataCriacao).IsRequired();
            builder.Property(p => p.Itens).IsRequired();
            builder.Property(p => p.ValorTotal).IsRequired();

            builder.HasOne<Cliente>()
                   .WithMany()
                   .HasForeignKey(p => p.ClienteId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
