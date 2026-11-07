using InvoiceManager.Api.DTOs.Invoices;
using InvoiceManager.Application.DTOs.Common;
using InvoiceManager.Application.DTOs.Invoices;
using InvoiceManager.Application.UseCases.Invoices;
using Microsoft.AspNetCore.Http;
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
        private readonly RegisterInvoicesUseCase _registerInvoicesUseCase;
        private readonly DesactiveInvoiceUseCase _desactiveInvoiceUseCase;
        private readonly GetInvoiceSummaryUseCase _getInvoiceSummaryUseCase;

        public InvoicesController(
            ListInvoicesUseCase listInvoicesUseCase,
            GetInvoiceDetailsUseCase getInvoiceDetailsUseCase,
            UpdateInvoiceUseCase updateInvoiceUseCase,
            DesactiveInvoiceUseCase desactiveInvoiceUseCase,
            GetInvoiceSummaryUseCase getInvoiceSummaryUseCase,
            RegisterInvoicesUseCase registerInvoicesUseCase
            )
        {
            _updateInvoiceUseCase = updateInvoiceUseCase;
            _getInvoiceDetailsUseCase = getInvoiceDetailsUseCase;
            _listInvoicesUseCase = listInvoicesUseCase;
            _desactiveInvoiceUseCase = desactiveInvoiceUseCase;
            _getInvoiceSummaryUseCase = getInvoiceSummaryUseCase;
            _registerInvoicesUseCase = registerInvoicesUseCase;
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
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(
        typeof(RegisteredInvoiceResult),
        StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<RegisteredInvoiceResult>> Register(
            [FromForm] UploadInvoiceForm request,
            CancellationToken cancellationToken)
        {
            if (request.File is null || request.File.Length == 0)
            {
                return BadRequest("Debe proporcionar un archivo válido.");
            }

            await using var fileStream = request.File.OpenReadStream();

            var registerRequest = new ReguisterInvoiceRequest(
                fileStream,
                request.File.FileName,
                request.File.ContentType,
                request.File.Length
            );

            var result = await _registerInvoicesUseCase.ExecuteAsync(
                registerRequest,
                cancellationToken
            );

            return StatusCode(
                StatusCodes.Status201Created,
                result
            );
        }
    }
}
