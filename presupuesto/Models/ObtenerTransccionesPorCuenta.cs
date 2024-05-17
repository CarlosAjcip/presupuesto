namespace presupuesto.Models
{
	public class ObtenerTransccionesPorCuenta
	{
        public int id_usuarios { get; set; }
        public int id_cuenta { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin {  get; set; }
    }
}
