using Dapper;
using Microsoft.Data.SqlClient;
using presupuesto.Models;
using System.Runtime.CompilerServices;

namespace presupuesto.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal MontoAnteriorerior, int cuentaAnteriorId);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
		Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransccionesPorCuenta obtener);
		Task<Transaccion> ObtenerPorId(int id_transacciones, int id_usuarios);
        Task<IEnumerable<Transaccion>> ObtenerPorIdUsuario(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int id_usuarios, int anio);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo);
    }
    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;

        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("defaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transaccion_Insertar",
                   new
                   {
                       transaccion.fechaTransaccion,
                       transaccion.monto,
                       transaccion.nota,
                       transaccion.id_usuarios,
                       transaccion.id_cuenta,
                       transaccion.id_categorias
                   },
                commandType: System.Data.CommandType.StoredProcedure);
            transaccion.id_transacciones = id;
        }
        public async Task Actualizar(Transaccion transaccion, decimal MontoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar",
                new
                {
                    transaccion.id_transacciones,
                    transaccion.fechaTransaccion,
                    transaccion.monto,
                    transaccion.id_categorias,
                    transaccion.id_cuenta,
                    transaccion.nota,
                    MontoAnterior,
                    cuentaAnteriorId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id_transacciones, int id_usuarios)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"SELECT Transacciones.*,cat.id_tiposOp FROM Transacciones 
            inner join categorias cat
            on cat.id_categorias = Transacciones.id_categorias
            WHERE Transacciones.id_transacciones = @id_transacciones AND Transacciones.id_usuarios = @id_usuarios" , new
            {
                id_transacciones, id_usuarios
            });
        }

        public async Task Borrar(int id_transacciones)
        {
            using var con = new SqlConnection(connectionString);
            await con.ExecuteAsync("Transacciones_Borrar",
                new { id_transacciones }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransccionesPorCuenta obtener)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryAsync<Transaccion>(@"select t.id_transacciones, t.monto,t.fechaTransaccion,c.Nombre as Categoria, ct.Nombre as Cuenta,
            c.id_tiposOp from Transacciones t
            inner join categorias c
            on c.id_categorias = t.id_categorias
            inner join Cuentas ct
            on ct.id_cuenta = t.id_cuenta
            where t.id_cuenta = @id_cuenta AND t.id_usuarios = @id_usuarios
            AND t.fechaTransaccion between @fechaInicio AND @fechaFin", obtener);
        }

        //reporte diario
        public async Task<IEnumerable<Transaccion>> ObtenerPorIdUsuario(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryAsync<Transaccion>(@"select t.id_transacciones, t.monto,t.fechaTransaccion,c.Nombre as Categoria, ct.Nombre as Cuenta,
            c.id_tiposOp, Nota from Transacciones t
            inner join categorias c
            on c.id_categorias = t.id_categorias
            inner join Cuentas ct
            on ct.id_cuenta = t.id_cuenta
            where t.id_usuarios = @id_usuarios  AND t.fechaTransaccion between @fechaInicio AND @fechaFin
            ORDER BY t.fechaTransaccion DESC", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryAsync<ResultadoObtenerPorSemana>(@"SELECT DATEDIFF(d,@fechaInicio,fechaTransaccion) / 7 + 1 as Semana, SUM(monto) as Monto, c.id_tiposOp as TipoOperacionId
            FROM Transacciones
            INNER JOIN categorias c
            ON c.id_categorias = Transacciones.id_categorias
            WHERE Transacciones.id_usuarios = @id_usuarios AND fechaTransaccion BETWEEN @fechaInicio AND @fechaFin
            GROUP BY DATEDIFF(d,@fechaInicio,fechaTransaccion) / 7, c.id_tiposOp", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int id_usuarios,int anio)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorMes>(@"
            SELECT MONTH(tr.fechaTransaccion) as Mes, SUM(monto) as Monto, ct.id_tiposOp FROM Transacciones tr
            inner join categorias ct
            on ct.id_categorias = tr.id_categorias
            where tr.id_usuarios = @id_usuarios AND YEAR(tr.fechaTransaccion) = @anio
            GROUP BY MONTH(tr.fechaTransaccion), ct.id_tiposOp", new { id_usuarios, anio });
        }
    }
}
