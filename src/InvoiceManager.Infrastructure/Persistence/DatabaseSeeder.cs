
using InvoiceManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    // Versión síncrona: utilizada por las herramientas de EF Core.
    public static void Seed(DbContext context)
    {
        if (context.Set<Invoice>().Any())
        {
            return;
        }

        var invoices = CreateSampleInvoices();

        context.Set<Invoice>().AddRange(invoices);

        context.SaveChanges();
    }

    // Versión asíncrona: utilizada por operaciones asíncronas.
    public static async Task SeedAsync(
        DbContext context,
        CancellationToken cancellationToken = default)
    {
        if (await context.Set<Invoice>().AnyAsync(cancellationToken))
        {
            return;
        }

        var invoices = CreateSampleInvoices();

        context.Set<Invoice>().AddRange(invoices);

        await context.SaveChangesAsync(cancellationToken);
    }

    // Construye las facturas y sus relaciones.
    private static List<Invoice> CreateSampleInvoices()
    {
        // Crear proveedores.
        var energySupplier =
            new Supplier("Energia del Norte S.A.S.");

        var waterSupplier =
            new Supplier("Acueducto Central S.A.");

        // Primera factura.
        var invoice1 = new Invoice(
            energySupplier,
            "EN-2026-00125",
            new DateOnly(2026, 10, 15),
            245800m,
            "COP",
            new DateOnly(2026, 10, 30),
            "CLI-00045"
        );

        // Segunda factura del mismo proveedor.
        var invoice2 = new Invoice(
            energySupplier,
            "EN-2026-00126",
            new DateOnly(2026, 10, 20),
            198450m,
            "COP",
            new DateOnly(2026, 11, 4),
            "CLI-00045"
        );

        // Tercera factura de otro proveedor.
        var invoice3 = new Invoice(
            waterSupplier,
            "AC-2026-00078",
            new DateOnly(2026, 10, 18),
            86400m,
            "COP",
            new DateOnly(2026, 10, 31),
            "CTA-00128"
        );

        // Respuestas ficticias originales del LLM.
        var response1 = """
        {
            "providerName": "Energia del Norte S.A.S.",
            "invoiceNumber": "EN-2026-00125",
            "issueDate": "2026-10-15",
            "dueDate": "2026-10-30",
            "totalAmount": 245800.00,
            "currency": "COP",
            "customerNumber": "CLI-00045"
        }
        """;

        var response2 = """
        {
            "providerName": "Energia del Norte S.A.S.",
            "invoiceNumber": "EN-2026-00126",
            "issueDate": "2026-10-20",
            "dueDate": "2026-11-04",
            "totalAmount": 198450.00,
            "currency": "COP",
            "customerNumber": "CLI-00045"
        }
        """;

        var response3 = """
        {
            "providerName": "Acueducto Central S.A.",
            "invoiceNumber": "AC-2026-00078",
            "issueDate": "2026-10-18",
            "dueDate": "2026-10-31",
            "totalAmount": 86400.00,
            "currency": "COP",
            "customerNumber": "CTA-00128"
        }
        """;

        // Crear los documentos relacionados.
        _ = new InvoiceDocument(
            invoice1,
            "factura-ejemplo-01.pdf",
            "application/pdf",
            1048576,
            "samples/invoices/factura-ejemplo-01.pdf",
            response1
        );

        _ = new InvoiceDocument(
            invoice2,
            "factura-ejemplo-02.pdf",
            "application/pdf",
            921600,
            "samples/invoices/factura-ejemplo-02.pdf",
            response2
        );

        _ = new InvoiceDocument(
            invoice3,
            "factura-ejemplo-03.jpg",
            "image/jpeg",
            768000,
            "samples/invoices/factura-ejemplo-03.jpg",
            response3
        );

        return
        [
            invoice1,
            invoice2,
            invoice3
        ];
    }
}
