using InvoiceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaController : ControllerBase
    {
        private readonly JsonLoaderService _jsonLoaderService;
        private readonly ILogger<FacturaController> _logger;

        public FacturaController(JsonLoaderService jsonLoaderService, ILogger<FacturaController> logger)
        {
            _jsonLoaderService = jsonLoaderService;
            _logger = logger;
        }

        /// <summary>
        /// Cargar facturas desde el archivo JSON.
        /// </summary>
        /// <returns>Mensaje de resultado.</returns>
        [HttpPost("cargar-json")]
        public async Task<IActionResult> CargarFacturas()
        {
            string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "DataFiles", "bd_exam.json");

            var resultMessage = await _jsonLoaderService.LoadInvoicesFromJsonAsync(jsonPath);

            _logger.LogInformation("Resultado de la carga: {Message}", resultMessage);

            return Ok(new { message = resultMessage });
        }
    }
}
