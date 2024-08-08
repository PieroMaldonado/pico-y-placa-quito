using Microsoft.AspNetCore.Mvc;
using PicoYPlacaQuito.Models;

namespace PicoYPlacaQuito.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PicoPlacaController : Controller
    {
        private readonly IConfiguration _configuration;

        public PicoPlacaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarConsulta([FromBody] ConsultaRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok("Mensaje");
        }
    }
}
