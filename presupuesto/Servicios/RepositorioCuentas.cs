using Dapper;
using Microsoft.Data.SqlClient;
using presupuesto.Models;

namespace presupuesto.Servicios
{
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionVMD cuenta);
        Task Borrar(int id_cuenta);
        Task<IEnumerable<Cuenta>> Buscar(int id_usuarios);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id_cuenta, int id_usuarios);
    }
    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string connectionString;
        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("defaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Cuentas (Nombre,id_TiposCuen,Descripcion,Balance)
                                                        VALUES (@Nombre,@id_TiposCuen,@Descripcion,@Balance)
                                                        SELECT SCOPE_IDENTITY();", cuenta);
            cuenta.id_cuenta = id;

        }

        public async Task<IEnumerable<Cuenta>> Buscar(int id_usuarios)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"select c.id_cuenta,c.Nombre, c.Balance,tc.Nombre as tipoCuenta, c.id_TiposCuen  from Cuentas c
                     inner join TiposCuentas tc
                     on tc.id_tiposCuen = c.id_TiposCuen
                     where tc.id_usuarios = @id_usuarios
                     order by tc.Orden", new { id_usuarios});
        }

        public async Task<Cuenta> ObtenerPorId(int id_cuenta, int id_usuarios)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"select c.id_cuenta,c.Nombre, c.Balance,Descripcion ,tc.id_tiposCuen  from Cuentas c
                inner join TiposCuentas tc
                on tc.id_tiposCuen = c.id_TiposCuen
                where tc.id_usuarios = @id_usuarios AND c.id_cuenta = @id_cuenta", new { id_cuenta, id_usuarios });
        }

        public async Task Actualizar(CuentaCreacionVMD cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Cuentas
            SET Nombre = @nombre, Balance = @Balance,Descripcion = @Descripcion, id_TiposCuen = @id_TiposCuen
            where id_cuenta = @id_cuenta", cuenta);
        }

        public async Task Borrar(int id_cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE Cuentas WHERE id_cuenta = @id_cuenta", new { id_cuenta });
        }
    }
}
