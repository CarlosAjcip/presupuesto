namespace presupuesto.Models
{
    public class IndiceCuentasVM
    {
        public string TipoCuenta { get; set; }
        public IEnumerable<Cuenta> Cuenta { get; set; }
        public decimal Balance => Cuenta.Sum(c => c.Balance);

    }
}
