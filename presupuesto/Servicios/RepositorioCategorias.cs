using Dapper;
using Microsoft.Data.SqlClient;
using presupuesto.Models;

namespace presupuesto.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id_categorias);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int id_usarios);
        Task<IEnumerable<Categoria>> Obtener(int id_usuarios, TipoOperacion id_tiposOp);
        Task<Categoria> ObtenerPorId(int id_categorias, int id_usuarios);
    }
    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string connectionString;
        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("defaultConnection");
        }

        public async Task<IEnumerable<Categoria>> Obtener(int id_usuarios)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>("SELECT * FROM categorias WHERE id_usuarios = @id_usuarios", new { id_usuarios });
        }

        public async Task<IEnumerable<Categoria>> Obtener(int id_usuarios, TipoOperacion id_tiposOp)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>("SELECT * FROM categorias WHERE id_usuarios = @id_usuarios AND id_tiposOp = @id_tiposOp", new { id_usuarios,
                id_tiposOp });
        }


        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO categorias(Nombre,id_usuarios,id_tiposOp)
                values(@Nombre,@id_usuarios,@id_tiposOp)
                SELECT SCOPE_IDENTITY()", categoria);

            categoria.id_categorias = id;
        }

        public async Task<Categoria> ObtenerPorId(int id_categorias, int id_usuarios)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(@"SELECT * FROM categorias WHERE id_categorias =@id_categorias AND id_usuarios = @id_usuarios ", 
                new { id_categorias, id_usuarios });
        }

        public async Task Actualizar(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("UPDATE categorias SET Nombre = @Nombre , id_tiposOp = @id_tiposOp WHERE id_categorias = @id_categorias", categoria);
        }

        public async Task Borrar(int id_categorias)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE categorias WHERE id_categorias = @id_categorias", new { id_categorias });
        }
    }
}
