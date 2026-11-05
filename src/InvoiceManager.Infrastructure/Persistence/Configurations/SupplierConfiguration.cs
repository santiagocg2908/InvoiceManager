using InvoiceManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceManager.Infrastructure.Persistence.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");
            //PrimaryKey
            builder.HasKey(supplier=>supplier.Id);

            builder.Property(supplier=>supplier.Name)
                .HasMaxLength(200)
                .IsRequired()
                .UsePropertyAccessMode(PropertyAccessMode.Field); // Forzar uso del campo privado


            builder.Property(supplier=>supplier.NormalizedName)
                .HasMaxLength(200)
                .IsRequired()
                .UsePropertyAccessMode(PropertyAccessMode.Field); // Forzar uso del campo privado

            builder.HasIndex(supplier => supplier.NormalizedName)
                .IsUnique()
                .HasDatabaseName("UX_Suppliers_NormalizedName");
        }
    }
}
