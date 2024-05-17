namespace presupuesto.Models
{
    public class Usuario
    {
        public int id_usuarios { get; set; }
        public string Email { get; set; }
        public string EmailNormalizado { get; set; }
        public string PaswordHash { get; set; }
    }
}
