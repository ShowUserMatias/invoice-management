using InvoiceApi.Data;
using InvoiceApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvoiceApi.Services;

namespace InvoiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaController : ControllerBase
    {
        private readonly JsonLoaderService _jsonLoaderService;
        private readonly InvoiceService _invoiceService;
        private readonly AppDbContext _context;
        private readonly ILogger<FacturaController> _logger;

        public FacturaController(JsonLoaderService jsonLoaderService, InvoiceService invoiceService ,AppDbContext context, ILogger<FacturaController> logger)
        {
            _jsonLoaderService = jsonLoaderService;
            _invoiceService = invoiceService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Cargar facturas desde el archivo JSON.
        /// </summary>
        [HttpPost("cargar-json")]
        public async Task<IActionResult> CargarFacturas()
        {
            string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "DataFiles", "bd_exam.json");

            var resultMessage = await _jsonLoaderService.LoadInvoicesFromJsonAsync(jsonPath);

            _logger.LogInformation("Resultado de la carga: {Message}", resultMessage);

            return Ok(new { message = resultMessage });
        }

        /// <summary>
        /// Buscar facturas por número, estado de factura y estado de pago.
        /// </summary>
        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarFacturas(
            [FromQuery] int? invoiceNumber,
            [FromQuery] string? invoiceStatus,
            [FromQuery] string? paymentStatus)
        {
            var query = _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.InvoiceDetails)
                .Include(i => i.InvoiceCreditNotes)
                .Include(i => i.InvoicePayment)
                .AsQueryable();

            // Filtro por número de factura
            if (invoiceNumber.HasValue)
            {
                query = query.Where(i => i.InvoiceNumber == invoiceNumber.Value);
            }

            if (!string.IsNullOrWhiteSpace(invoiceStatus))
            {
                var normalizedInvoiceStatus = invoiceStatus.ToLower();
                query = query.Where(i => i.InvoiceStatus.ToLower() == normalizedInvoiceStatus);
            }
            
            if (!string.IsNullOrWhiteSpace(paymentStatus))
            {
                var normalizedPaymentStatus = paymentStatus.ToLower();
                query = query.Where(i => i.PaymentStatus.ToLower() == normalizedPaymentStatus);
            }

            var results = await query
        .Select(i => new InvoiceDto
        {
            InvoiceId = i.InvoiceId,
            InvoiceNumber = i.InvoiceNumber,
            InvoiceDate = i.InvoiceDate,
            TotalAmount = i.TotalAmount,
            PaymentDueDate = i.PaymentDueDate,
            PaymentStatus = i.PaymentStatus,
            InvoiceStatus = i.InvoiceStatus,
            Customer = new CustomerDto
            {
                CustomerRun = i.Customer.CustomerRun,
                CustomerName = i.Customer.CustomerName,
                CustomerEmail = i.Customer.CustomerEmail
            },
            InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailDto
            {
                ProductName = d.ProductName,
                UnitPrice = d.UnitPrice,
                Quantity = d.Quantity,
                Subtotal = d.Subtotal
            }).ToList(),
            InvoiceCreditNotes = i.InvoiceCreditNotes.Select(nc => new InvoiceCreditNoteDto
            {
                CreditNoteNumber = nc.CreditNoteNumber,
                CreditNoteDate = nc.CreditNoteDate,
                CreditNoteAmount = nc.CreditNoteAmount
            }).ToList(),
            InvoicePayment = i.InvoicePayment == null ? null : new InvoicePaymentDto
            {
                PaymentMethod = i.InvoicePayment.PaymentMethod,
                PaymentDate = i.InvoicePayment.PaymentDate
            }
        })
        .ToListAsync();

    return Ok(results);
}

        /// <summary>
        /// Agregar una Nota de Crédito a una factura.
        /// </summary>
        [HttpPost("agregar-nc")]
        public async Task<IActionResult> AgregarNotaDeCredito([FromBody] NotaCreditoRequest request)
        {
            if (request.CreditNoteAmount <= 0)
            {
                return BadRequest(new { message = "El monto de la nota de crédito debe ser mayor a cero." });
            }

            var resultMessage = await _invoiceService.AddCreditNoteAsync(
                request.InvoiceId,
                request.CreditNoteNumber,
                request.CreditNoteAmount
            );

            if (resultMessage.Contains("exitosamente"))
            {
                return Ok(new { message = resultMessage });
            }
            else
            {
                return BadRequest(new { message = resultMessage });
            }

        }
            /// <summary>
            /// Obtener detalles de una factura por su ID.
            /// </summary>
            [HttpGet("{invoiceId}")]
            public async Task<IActionResult> ObtenerFacturaPorId(int invoiceId)
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceDetails)
                    .Include(i => i.InvoiceCreditNotes)
                    .Include(i => i.InvoicePayment)
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                if (invoice == null)
                    return NotFound(new { message = "Factura no encontrada." });

                    var invoiceDto = new InvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                TotalAmount = invoice.TotalAmount,
                PaymentDueDate = invoice.PaymentDueDate,
                PaymentStatus = invoice.PaymentStatus,
                InvoiceStatus = invoice.InvoiceStatus,
                Customer = new CustomerDto
                {
                    CustomerRun = invoice.Customer.CustomerRun,
                    CustomerName = invoice.Customer.CustomerName,
                    CustomerEmail = invoice.Customer.CustomerEmail
                },
                InvoiceDetails = invoice.InvoiceDetails.Select(d => new InvoiceDetailDto
                {
                    ProductName = d.ProductName,
                    UnitPrice = d.UnitPrice,
                    Quantity = d.Quantity,
                    Subtotal = d.Subtotal
                }).ToList(),
                InvoiceCreditNotes = invoice.InvoiceCreditNotes.Select(nc => new InvoiceCreditNoteDto
                {
                    CreditNoteNumber = nc.CreditNoteNumber,
                    CreditNoteDate = nc.CreditNoteDate,
                    CreditNoteAmount = nc.CreditNoteAmount
                }).ToList(),
                InvoicePayment = invoice.InvoicePayment == null ? null : new InvoicePaymentDto
                {
                    PaymentMethod = invoice.InvoicePayment.PaymentMethod,
                    PaymentDate = invoice.InvoicePayment.PaymentDate
                }
            };

            return Ok(invoiceDto);
        }
    }

    /// <summary>
    /// Modelo de solicitud para agregar una Nota de Crédito.
    /// </summary>
    public class NotaCreditoRequest
    {
        public int InvoiceId { get; set; }
        public int CreditNoteNumber { get; set; }
        public decimal CreditNoteAmount { get; set; }
    }
}
    
