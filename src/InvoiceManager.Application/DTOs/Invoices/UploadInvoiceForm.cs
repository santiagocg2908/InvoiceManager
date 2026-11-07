using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace InvoiceManager.Api.DTOs.Invoices;

public sealed class UploadInvoiceForm
{
    [Required]
    public IFormFile File { get; set; } = null!;
}