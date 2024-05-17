using Dapper;
using Microsoft.Data.SqlClient;
using presupuesto.Models;

namespace presupuesto.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario> BuscarUsuarioPorEmail(string EmailNormalizado);
        Task<int> CrarUsuario(Usuario usuario);
    }
    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly string ConnectionString;

        public RepositorioUsuarios(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("defaultConnection");
        }

        public async Task<int> CrarUsuario(Usuario usuario)
        {
            //usuario.EmailNormalizado = usuario.Email.ToUpper();
            using var connection = new SqlConnection(ConnectionString);

            var id_usuarios = await connection.QuerySingleAsync<int>(@"INSERT INTO usuarios (Email,EmailNormalizado,PaswordHash)
            values (@Email,@EmailNormalizado,@PaswordHash);
            SELECT SCOPE_IDENTITY();", usuario);

            await connection.ExecuteAsync("CrearDatosUsuarioNuevo", new {id_usuarios }, commandType: System.Data.CommandType.StoredProcedure);
            return id_usuarios;
        }

        public async Task<Usuario> BuscarUsuarioPorEmail(string EmailNormalizado)
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.QuerySingleOrDefaultAsync<Usuario>(@"SELECT * FROM usuarios where EmailNormalizado = @EmailNormalizado", new { EmailNormalizado });
        }
    }
}
