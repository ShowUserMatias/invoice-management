using InvoiceApi.Data;
using InvoiceApi.Models;
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
        private readonly AppDbContext _context;
        private readonly ILogger<FacturaController> _logger;

        public FacturaController(JsonLoaderService jsonLoaderService, AppDbContext context, ILogger<FacturaController> logger)
        {
            _jsonLoaderService = jsonLoaderService;
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

            if (invoiceNumber.HasValue)
            {
                query = query.Where(i => i.InvoiceNumber == invoiceNumber.Value);
            }

            if (!string.IsNullOrWhiteSpace(invoiceStatus))
            {
                query = query.Where(i => i.InvoiceStatus.Equals(invoiceStatus, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(paymentStatus))
            {
                query = query.Where(i => i.PaymentStatus.Equals(paymentStatus, StringComparison.OrdinalIgnoreCase));
            }

            var results = await query.ToListAsync();

            return Ok(results);
        }
    }
}
