using Microsoft.AspNetCore.Mvc.RazorPages;

namespace presupuesto.Models
{
	public class ReporteTransccionesDetallas
	{
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public IEnumerable<TranaccionesPorFecha> TransaccionesAgrupadas { get; set; }
        public decimal BalanceDepositos => TransaccionesAgrupadas.Sum(x => x.BalanceDepositos);
        public decimal BalanceRetiros => TransaccionesAgrupadas.Sum(x => x.BalanceRetiros);
        public decimal Total => BalanceDepositos - BalanceRetiros;

        public class TranaccionesPorFecha
        {
            public DateTime FechaTransaccion { get; set; }
            public IEnumerable<Transaccion> Transacciones { get; set; }
            public decimal BalanceDepositos => 
                Transacciones.Where(x => x.id_TiposOp == TipoOperacion.Ingreso)
                .Sum(x => x.monto);

			public decimal BalanceRetiros =>
			   Transacciones.Where(x => x.id_TiposOp == TipoOperacion.Gastos)
			   .Sum(x => x.monto);
		}
    }
}
