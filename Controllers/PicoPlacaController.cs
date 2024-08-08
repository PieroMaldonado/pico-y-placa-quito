using Microsoft.AspNetCore.Mvc;
using PicoYPlacaQuito.Models;
using Microsoft.Data.SqlClient;
using System.Data;

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

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            { 
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("RegistrarConsulta", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Placa", request.Placa);
                    command.Parameters.AddWithValue("@Fecha", request.Fecha.Date);
                    command.Parameters.AddWithValue("@Hora", request.Hora);

                    bool puedeCircular = DeterminarSiPuedeCircular(request.Placa, request.Fecha, request.Hora);
                    command.Parameters.AddWithValue("@PuedeCircular", puedeCircular);
                    await command.ExecuteNonQueryAsync();
                    var response = new ConsultaResponse
                    {
                        PuedeCircular = puedeCircular,
                        Mensaje = puedeCircular ? "El vehículo puede circular." : "El vehículo no puede circular."
                    };

                    return Ok(response);
                }
            }
        }

        private bool DeterminarSiPuedeCircular(string placa, DateTime fecha, TimeSpan hora)
        {
            // Extraer el último dígito de la placa
            char ultimoDigito = placa[^1];

            // Determinar el día de la semana
            DayOfWeek diaSemana = fecha.DayOfWeek;

            // Verificar si es sábado, domingo
            if (diaSemana == DayOfWeek.Saturday || diaSemana == DayOfWeek.Sunday)
            {
                return true; // Libre circulación en fines de semana
            }

            // Verificar los dígitos permitidos para el día actual
            bool puedeCircularHoy = diaSemana switch
            {
                DayOfWeek.Monday => ultimoDigito == '1' || ultimoDigito == '2',
                DayOfWeek.Tuesday => ultimoDigito == '3' || ultimoDigito == '4',
                DayOfWeek.Wednesday => ultimoDigito == '5' || ultimoDigito == '6',
                DayOfWeek.Thursday => ultimoDigito == '7' || ultimoDigito == '8',
                DayOfWeek.Friday => ultimoDigito == '9' || ultimoDigito == '0',
                _ => true // No restringido para otros días
            };

            if (puedeCircularHoy)
            {
                // Definir horarios mañana y tarde restringidos
                TimeSpan horaInicioManana = new TimeSpan(6, 0, 0);
                TimeSpan horaFinManana = new TimeSpan(9, 30, 0);
                TimeSpan horaInicioTarde = new TimeSpan(16, 0, 0);
                TimeSpan horaFinTarde = new TimeSpan(20, 0, 0);

                bool enHorarioRestringido = (hora >= horaInicioManana && hora <= horaFinManana) ||
                                            (hora >= horaInicioTarde && hora <= horaFinTarde);

                return !enHorarioRestringido; // Puede circular si no está en horario restringido
            }

            return true; // Puede circular hoy según el último dígito
        }


    }
}
