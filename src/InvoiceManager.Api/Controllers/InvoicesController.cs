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
        private readonly UpdateInvoiceUseCase _updateInvoiceUseCase;

        private readonly DesactiveInvoiceUseCase _desactiveInvoiceUseCase;
        private readonly GetInvoiceSummaryUseCase _getInvoiceSummaryUseCase;

        public InvoicesController(
            ListInvoicesUseCase listInvoicesUseCase,
            GetInvoiceDetailsUseCase getInvoiceDetailsUseCase,
            UpdateInvoiceUseCase updateInvoiceUseCase,
            DesactiveInvoiceUseCase desactiveInvoiceUseCase,
            GetInvoiceSummaryUseCase getInvoiceSummaryUseCase
            )
        {
            _updateInvoiceUseCase = updateInvoiceUseCase;
            _getInvoiceDetailsUseCase = getInvoiceDetailsUseCase;
            _listInvoicesUseCase = listInvoicesUseCase;
            _desactiveInvoiceUseCase = desactiveInvoiceUseCase;
            _getInvoiceSummaryUseCase = getInvoiceSummaryUseCase;
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
        public async Task<ActionResult<InvoiceDetailDto>> GetById(
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

        [HttpPut("{id:int}")]
        [ProducesResponseType(
            typeof(InvoiceDetailDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<InvoiceDetailDto>> Update
            (
            [FromRoute] int id,
            [FromBody] UpdateInvoiceRequest request,
            CancellationToken cancellationToken
            )
        {
            var result = await _updateInvoiceUseCase.ExecuteAsync(
                id,
                request,
                cancellationToken
                );

            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Desactive(
            int id,
            CancellationToken cancellationToken
            )
        {
            bool desactived =
                await _desactiveInvoiceUseCase.ExecuteAsync(
                    id, cancellationToken
                    );

            if (!desactived)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("reporte/resumen")]
        [ProducesResponseType(
            typeof(IReadOnlyList<InvoiceSummaryDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<InvoiceSummaryDto>>> GetSummary(
            CancellationToken cancellationToken)
        {
            var result = await _getInvoiceSummaryUseCase.ExecuteAsync(
                cancellationToken
                );
            return Ok(result);
        }
    }
}
