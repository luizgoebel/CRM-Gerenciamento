using CRM.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.EntityConfigurations;

public class ClienteEntityTypeConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("cliente");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome).IsRequired();
        builder.Property(c => c.Telefone).IsRequired();
        builder.Property(c => c.Email).IsRequired();
        builder.Property(c => c.Endereco).IsRequired();
    }
}