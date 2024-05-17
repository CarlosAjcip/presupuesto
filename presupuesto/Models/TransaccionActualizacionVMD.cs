namespace presupuesto.Models
{
    public class TransaccionActualizacionVMD : TransaccionCreacionVMD
    {
        public int CuentaAnteriorId { get; set; }
        public decimal MontoAnterior {  get; set; }
        public string UrlRetorno { get; set; }      
    }
}
