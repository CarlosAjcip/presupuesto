namespace presupuesto.Models
{
    public class ResultadoObtenerPorMes
    {
        public int Mes { get; set; }
        public DateTime FechaReferencia { get; set; }
        public decimal Monto { get; set; }
        public decimal Ingreso { get; set; }
        public decimal Gasto { get; set; }
        public TipoOperacion id_tiposOp { get; set; }
    }
}
