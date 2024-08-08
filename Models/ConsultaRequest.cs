using System.ComponentModel.DataAnnotations;

namespace PicoYPlacaQuito.Models
{
    public class ConsultaRequest
    {
        [Required(ErrorMessage = "La placa es obligatoria.")]
        [StringLength(7, MinimumLength = 6, ErrorMessage = "La placa debe tener exactamente 7 caracteres.")]
        [RegularExpression(@"^[A-Z]{3}\d{3,4}$", ErrorMessage = "La placa debe seguir el formato ABC123 o ABC1234")]


        public string Placa { get; set; }
        [Required(ErrorMessage = "La fecha es obligatoria.")]

        public DateTime Fecha { get; set; }
        [Required(ErrorMessage = "La hora es obligatoria.")]

        public TimeSpan Hora { get; set; }
    }
}
