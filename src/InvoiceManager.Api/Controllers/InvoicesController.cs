using InvoiceManager.Application.DTOs.Common;
using InvoiceManager.Application.DTOs.Invoices;
using InvoiceManager.Application.UseCases.Invoices;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManager.Api.Controllers
{
    [ApiController]
    [Route("api/facturas")]
    public class InvoicesController : ControllerBase
    {
        private readonly ListInvoicesUseCase _listInvoicesUseCase;

        public InvoicesController(
            ListInvoicesUseCase listInvoicesUseCase
            )
        {
            _listInvoicesUseCase = listInvoicesUseCase;
        }

        [HttpGet]
        [ProducesResponseType(
            typeof(PagedResult<InvoiceListItemDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<InvoiceListItemDto>>>
            GetAll(
                [FromQuery] InvoiceListQuery query,
                CancellationToken cancellationToken)
        {
            var result = await _listInvoicesUseCase.ExecuteAsync(
                query,
                cancellationToken
            );

            return Ok(result);
        }
    }
}
