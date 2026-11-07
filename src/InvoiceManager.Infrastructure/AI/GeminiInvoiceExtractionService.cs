using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;//Serialized and Deserialized the Json
using InvoiceManager.Application.Abstractions.Extraction;
using InvoiceManager.Application.DTOs.Invoices;
using InvoiceManager.Application.Exceptions;
using Microsoft.Extensions.Options;

namespace InvoiceManager.Infrastructure.AI
{
    public sealed class GeminiInvoiceExtractionService : IInvoiceExtractionService
    {
        private const long MaxFileSizeBytes = 10L * 1024 * 1024;
        private readonly HttpClient _httpClient;
        private readonly GeminiOptions _options;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public GeminiInvoiceExtractionService(HttpClient httpClient,
            IOptions <GeminiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<InvoiceExtractionResult> ExtractAsync(Stream document,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(document);

            if (!document.CanRead) 
            {
                throw new InvalidInvoiceFileException(
                    "The document can be read"
                    );
            }

            if (string.IsNullOrWhiteSpace(_options.ApiKey)) 
            {
                throw new InvalidOperationException(
                    "The Gemini API Key is not configured"
                    );
            }

            using var memoryStream = new MemoryStream();

            if (document.CanSeek)
            {
                document.Position = 0;
            }

            byte[] buffer = new byte[81920];

            int bytesRead;

            while ((bytesRead = await document.ReadAsync
                (buffer.AsMemory(0,buffer.Length), cancellationToken))>0)
            {
                if (memoryStream.Length + bytesRead > MaxFileSizeBytes)
                {
                    throw new InvoiceFileTooLargeException(
                        "The document exceed the limit of 10MB"
                        );
                }

                await memoryStream.WriteAsync(
                    buffer.AsMemory(0, bytesRead),
                    cancellationToken
                    );
            }
            if (memoryStream.Length == 0)
            {
                throw new InvalidInvoiceFileException(
                    "The document is empty"
                    );
            }

            string base64Document = Convert.ToBase64String(
                memoryStream.ToArray()
                );

            string prompt = BuildPrompt();

            object schema = BuildResponseSchema();
            
            //build the gemini request
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new
                            {
                                text = prompt
                            },
                            new
                            {
                                inlineData = new
                                {
                                    mimeType = contentType,
                                    data = base64Document
                                }
                            }
                        }
                    }
                },
                generationConfig = new
                {
                    responseMimeType = "application/json",
                    responseSchema = schema,
                    temperature = 0.1
                }
            };

            //Send the HTTP request
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"v1beta/models/{_options.Model}:generateContent"
                );

            request.Headers.Add(
                    "x-goog-api-key",
                    _options.ApiKey
                );
            request.Content = JsonContent.Create(requestBody);

            using var response = await _httpClient.SendAsync( 
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
                );

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync(
                    cancellationToken
                    );
                throw new HttpRequestException(
                        $"Gemini devolvió el estado HTTP {(int)response.StatusCode}. " +
                        $"Detalle: {errorBody}",
                        null,
                        response.StatusCode
                    );
            }
            using var responseStream = await response.Content.ReadAsStreamAsync(
                cancellationToken
                );

            using var jsonDocument = await JsonDocument.ParseAsync(
                responseStream,
                cancellationToken: cancellationToken
            );

            JsonElement root = jsonDocument.RootElement;

            if (!root.TryGetProperty("candidates", out var candidates) ||
                candidates.GetArrayLength() == 0)
            {
                throw new InvalidInvoiceExtractionException(
                    "Gemini dont return candidates to extraction.");
            }
            JsonElement candidate = candidates[0];

            if (candidate.TryGetProperty("finishReason", out var finishReason) &&
                finishReason.GetString() != "STOP")
            {
                throw new InvalidInvoiceExtractionException(
                    "Gemini did not complete the extraction. reason: " +
                    finishReason.GetString());
            }

            if (!candidate.TryGetProperty("content", out var content) ||
                !content.TryGetProperty("parts", out var parts))
            {
                throw new InvalidInvoiceExtractionException(
                    "The Gemini answer is empty");
            }
            string? extractedJson = parts
                .EnumerateArray()
                .Where(part => part.TryGetProperty("text", out _))
                .Select(part => part.GetProperty("text").GetString())
                .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));
            if (string.IsNullOrWhiteSpace(extractedJson))
            {
                throw new InvalidInvoiceExtractionException(
                    "The Gemini answer is empty");
            }

            var extractedData =
                JsonSerializer.Deserialize<GeminiInvoiceData>(
                    extractedJson,
                    JsonOptions
                );

            if (extractedData is null)
            {
                throw new InvalidInvoiceExtractionException(
                    "it was not posible interpreter the extraction of gemini.");
            }
            if (!DateOnly.TryParseExact(
                extractedData.IssueDate,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateOnly issueDate))
            {
                throw new InvalidInvoiceExtractionException(
                    "Gemini returned an invalid issuance date.");
            }
            DateOnly? dueDate = null;

            if (!string.IsNullOrWhiteSpace(extractedData.DueDate))
            {
                if (!DateOnly.TryParseExact(
                    extractedData.DueDate,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateOnly parsedDueDate))
                {
                    throw new InvalidInvoiceExtractionException(
                        "Gemini returned an invalid Due Date");
                }

                dueDate = parsedDueDate;
            }
            return new InvoiceExtractionResult(
                extractedData.ProviderName,
                extractedData.InvoiceNumber,
                issueDate,
                dueDate,
                extractedData.TotalAmount,
                extractedData.Currency,
                string.IsNullOrWhiteSpace(
                    extractedData.CustomerOrContractNumber)
                        ? null
                        : extractedData.CustomerOrContractNumber,
                extractedJson
                );
        }

        private static object BuildResponseSchema()
        {
            return new
            {
                type = "OBJECT",
                properties = new
                {
                    providerName = new
                    {
                        type = "STRING",
                        description = "Nombre legal del proveedor."
                    },
                    invoiceNumber = new
                    {
                        type = "STRING",
                        description = "Número de identificación de la factura."
                    },
                    issueDate = new
                    {
                        type = "STRING",
                        description = "Fecha de emisión en formato yyyy-MM-dd."
                    },
                    dueDate = new
                    {
                        type = "STRING",
                        description = "Fecha de vencimiento yyyy-MM-dd o cadena vacía."
                    },
                    totalAmount = new
                    {
                        type = "NUMBER",
                        description = "Valor total de la factura."
                    },
                    currency = new
                    {
                        type = "STRING",
                        description = "Código de moneda de tres letras."
                    },
                    customerOrContractNumber = new
                    {
                        type = "STRING",
                        description = "Número de cliente o contrato, o cadena vacía."
                    }
                },
                required = new[]
                {
                "providerName",
                "invoiceNumber",
                "issueDate",
                "dueDate",
                "totalAmount",
                "currency",
                "customerOrContractNumber"
            },
            };
        }

        private string BuildPrompt()
        {
            return """
            Actúa como un sistema especializado en extracción de datos
            de facturas comerciales.

            Analiza el documento adjunto y extrae exclusivamente
            la información que pueda identificarse en él.

            Reglas:

            1. Identifica el nombre del proveedor o emisor.
            2. Identifica el número de factura.
            3. Extrae la fecha de emisión en formato yyyy-MM-dd.
            4. Extrae la fecha de vencimiento en formato yyyy-MM-dd.
            5. Si no existe fecha de vencimiento, devuelve una cadena vacía.
            6. Extrae el valor total a pagar como número decimal.
            7. Identifica la moneda mediante su código de tres letras.
            8. Extrae el número de cliente o contrato si existe.
            9. Si no existe número de cliente o contrato, devuelve una
               cadena vacía.
            10. No inventes información.
            11. No confundas la fecha de emisión con la fecha de vencimiento.
            12. Utiliza el total final de la factura, no el subtotal.
            13. Si un dato obligatorio no puede identificarse,
                no lo inventes. Devuelve un valor vacío o cero
                según corresponda para que la aplicación lo rechace.

            Devuelve únicamente los datos solicitados siguiendo
            el esquema JSON proporcionado.
            """;
        }
        private sealed class GeminiInvoiceData
        {
            public string ProviderName { get; set; } = string.Empty;

            public string InvoiceNumber { get; set; } = string.Empty;

            public string IssueDate { get; set; } = string.Empty;

            public string DueDate { get; set; } = string.Empty;

            public decimal TotalAmount { get; set; }

            public string Currency { get; set; } = string.Empty;

            public string CustomerOrContractNumber { get; set; }
                = string.Empty;
        }
    }

}
