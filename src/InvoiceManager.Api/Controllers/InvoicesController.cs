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
        private readonly GetInvoiceDetailsUseCase _getInvoiceDetailsUseCase;

        public InvoicesController(
            ListInvoicesUseCase listInvoicesUseCase,
            GetInvoiceDetailsUseCase getInvoiceDetailsUseCase
            )
        {
            _getInvoiceDetailsUseCase = getInvoiceDetailsUseCase;
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

        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(InvoiceDetailDto),
            StatusCodes.Status200OK)
            ]
        [ProducesResponseType(
            StatusCodes.Status404NotFound
            )]
        public async Task <ActionResult<InvoiceDetailDto>> GetById(
            int id,
            CancellationToken cancellationToken
            )
        {
            var result = await _getInvoiceDetailsUseCase.ExecuteAsync(
                id,
                cancellationToken
                );

            if (result is null) return NotFound();

            return Ok(result);
        }
    }
}
