using InvoiceManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace InvoiceManager.Infrastructure.Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices", table =>
            {
                table.HasCheckConstraint(
                    "CK_Invoices_TotalAmount_Positive",
                    "[TotalAmount] > 0");
            });
            //PrimaryKey
            builder.HasKey(invoice => invoice.Id);

            builder.Property(invoice => invoice.InvoiceNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(invoice => invoice.NormalizedInvoiceNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(invoice => invoice.IssueDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(invoice => invoice.DueDate)
                .HasColumnType("date");

            builder.Property(invoice => invoice.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(invoice => invoice.Currency)
                .HasColumnType("char(3)")
                .IsRequired();

            builder.Property(invoice => invoice.CustomerOrContactNumber)
                .HasMaxLength(100);

            builder.Property(invoice => invoice.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(invoice => invoice.CreatedAtUtc)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.HasIndex(invoice => new
            {
                invoice.SupplierId,
                invoice.NormalizedInvoiceNumber
            })
                .IsUnique()
                .HasDatabaseName("UX_Invoices_Supplier_InvoiceNumber");
            //Relationship: Supplier to Invoice: 1/N
            builder.HasOne(invoce => invoce.Supplier)
                .WithMany(supplier => supplier.Invoices)
                .HasForeignKey(invoice => invoice.SupplierId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            //Relationship: Invoice to Document: 1/1
            builder.HasOne(invoice => invoice.Document)
                .WithOne(document => document.Invoice)
                .HasForeignKey<InvoiceDocument>(
                    document => document.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
