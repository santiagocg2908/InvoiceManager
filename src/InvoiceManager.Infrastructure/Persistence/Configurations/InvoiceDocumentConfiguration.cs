using InvoiceManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceManager.Infrastructure.Persistence.Configurations
{
    public class InvoiceDocumentConfiguration : IEntityTypeConfiguration<InvoiceDocument>
    {
        public void Configure(EntityTypeBuilder<InvoiceDocument> builder)
        {
            //Table name and size restriction
            builder.ToTable("InvoiceDocuments", table=>
            {
                table.HasCheckConstraint(
                    "CK_InvoiceDocuments_SizeBytes_Positive",
                    "[SizeBytes] > 0");
            });
            //Primary Key
            builder.HasKey(document => document.Id);

            builder.Property(document => document.InvoiceId)
                .IsRequired();

            builder.Property(document => document.OriginalFileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(document => document.SizeBytes)
                .IsRequired();

            builder.Property(document => document.UploadedAtUtc)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(document => document.StoragePath)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(document => document.OriginalLlmResponseJson)
                .HasColumnType("nvarchar(max)")
                .IsRequired();
        }
    }
}
