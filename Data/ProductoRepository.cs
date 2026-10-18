using CrudProductosOracle19c.Models;
using Oracle.ManagedDataAccess.Client;  

namespace CrudProductosOracle19c.Data
{
    public class ProductoRepository
    {
        private readonly string _connectionString;

        public ProductoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb")
                ?? throw new InvalidOperationException("falta ConnectionStrings: OracleDb en appsettings.json!");
        }

        private OracleConnection CrearConexion() => new(_connectionString);

        public async Task<List<Producto>> ObtenerTodosAsync(string? buscar = null)
        {
            var productos = new List<Producto>();
            await using var cn = CrearConexion();
            await cn.OpenAsync();

            const string sql = """
                SELECT ID, NOMBRE, DESCRIPCION, PRECIO, STOCK, CATEGORIA, FECHA_CREACION
                FROM PRODUCTOS
                WHERE (:p_buscar IS NULL 
                OR UPPER(NOMBRE) LIKE '%' || UPPER(:p_buscar) || '%'
                OR UPPER(CATEGORIA) LIKE '%' || UPPER(:p_buscar) || '%')
                ORDER BY ID DESC
                """;

            await using var cmd = new OracleCommand(sql, cn);
            cmd.BindByName = true;
            cmd.Parameters.Add("p_buscar", OracleDbType.Varchar2).Value = string.IsNullOrWhiteSpace(buscar) ? DBNull.Value : buscar.Trim();

            return productos;
        }

        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            await using var cn = CrearConexion();
            await cn.OpenAsync();

            const string sql = """
                SELECT ID, NOMBRE, DESCRIPCION, PRECIO, STOCK, CATEGORIA, FECHA_CREACION
                FROM PRODUCTOS
                WHERE ID = :p_id
                """;

            await using var cmd = new OracleCommand(sql, cn);
            cmd.BindByName = true;
            cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = id;

            await using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? LeerProducto(reader) : null;
        }

        public async Task CrearAsync(Producto producto)
        {
            await using var cn = CrearConexion();
            await cn.OpenAsync();

            const string sql = """
                
                INSERT INTO PRODUCTOS (NOMBRE, DESCRIPCION, PRECIO, STOCK, CATEGORIA)
                VALUES (:p_nombre, :p_descripcion, :p_precio, :p_stock, :p_categoria)
                """;

            await using var cmd = new OracleCommand(sql, cn);
            cmd.BindByName = true;
            AgregarParametrosProducto(cmd, producto);
            await cmd.ExecuteNonQueryAsync();
        }

        private void AgregarParametrosProducto(OracleCommand cmd, Producto producto)
        {
            throw new NotImplementedException();
        }

        private Producto? LeerProducto(OracleDataReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
    
