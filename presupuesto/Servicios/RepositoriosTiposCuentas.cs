using Dapper;
using Microsoft.Data.SqlClient;
using presupuesto.Models;

namespace presupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TiposCuentas tiposCuentas);
        Task Borrar(int id_tiposCuen);
        Task Crear(TiposCuentas tipoCuenta);
        Task<bool> Existe(string nombre, int id_usuario);
        Task<IEnumerable<TiposCuentas>> Obtener(int id_usuarios);
        Task<TiposCuentas> ObtenerPorId(int id_tiposCuen, int id_usuarios);
        Task Ordenar(IEnumerable<TiposCuentas> tiposCuentasOrdenados);
    }

    public class RepositoriosTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;

        public RepositoriosTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("defaultConnection");
        }

        public async Task<IEnumerable<TiposCuentas>> Obtener(int id_usuarios)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TiposCuentas>(@"SELECT id_tiposCuen,Nombre,Orden FROM TiposCuentas
                                                                where id_usuarios = @id_usuarios ORDER BY orden", new {id_usuarios});
        }

        public async Task<bool> Existe(string nombre, int id_usuario)
        {
            using var connectino = new SqlConnection(connectionString);
            var existe = await connectino.QueryFirstOrDefaultAsync<int>(@"SELECT 1 FROM TiposCuentas
                                                                        WHERE Nombre = @nombre AND id_usuarios = @id_usuario", new { nombre, id_usuario });
            return existe == 1;
        }
        public async Task Crear(TiposCuentas tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>
                 ("TiposCuentasInsertar",
                 new
                 {
                     id_usuarios = tipoCuenta.id_usuarios,
                     Nombre = tipoCuenta.Nombre
                 },
                 commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.id_tiposCuen = id;
                    
        }

        public async Task Actualizar(TiposCuentas tiposCuentas)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas SET Nombre = @nombre 
                                            WHERE id_tiposCuen = @id_tiposCuen", tiposCuentas);
        }


        public async Task<TiposCuentas> ObtenerPorId(int id_tiposCuen, int id_usuarios)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TiposCuentas>(@"SELECT id_tiposCuen,Nombre,Orden FROM TiposCuentas
                                                            WHERE id_tiposCuen = @id_tiposCuen AND id_usuarios = @id_usuarios",
                                                            new { id_tiposCuen, id_usuarios });
        }

        public async Task Borrar(int id_tiposCuen)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE TiposCuentas WHERE id_tiposCuen = @id_tiposCuen", new { id_tiposCuen });
        }

        public async Task Ordenar(IEnumerable<TiposCuentas> tiposCuentasOrdenados)
        {
            var query = "UPDATE TiposCuentas SET Orden = @Orden where id_tiposCuen = @id_tiposCuen";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tiposCuentasOrdenados);
        }
    }
}
