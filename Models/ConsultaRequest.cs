namespace PicoYPlacaQuito.Models
{
    public class ConsultaRequest
    {
        public string Placa { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
    }
}
