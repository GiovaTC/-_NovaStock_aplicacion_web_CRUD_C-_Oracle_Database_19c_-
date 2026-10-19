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
                FROM PRODUCTOS_L
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
                FROM PRODUCTOS_L
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
                
                INSERT INTO PRODUCTOS_L (NOMBRE, DESCRIPCION, PRECIO, STOCK, CATEGORIA)
                VALUES (:p_nombre, :p_descripcion, :p_precio, :p_stock, :p_categoria)
                """;

            await using var cmd = new OracleCommand(sql, cn);
            cmd.BindByName = true;
            AgregarParametrosProducto(cmd, producto);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> ActualizarAsync(Producto producto)
        {
            await using var cn = CrearConexion();
            await cn.OpenAsync();

            const string sql = """
                UPDATE PRODUCTOS_L
                SET NOMBRE = :p_nombre,
                    DESCRIPCION = :p_descripcion,
                    PRECIO = :p_precio,
                    STOCK = :p_stock,
                    CATEGORIA = :p_categoria
                WHERE ID = :p_id
                """;

            await using var cmd = new OracleCommand(sql, cn);
            cmd.BindByName = true;
            AgregarParametrosProducto(cmd, producto);
            cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = producto.Id;

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        
        public async Task<bool> EliminarAsync(int id)
        {
            await using var cn = CrearConexion();
            await cn.OpenAsync();

            await using var cmd = 
                new OracleCommand("DELETE FROM PRODUCTOS_L WHERE ID = :p_id", cn);
            cmd.BindByName = true;
            cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
            
            return await cmd.ExecuteNonQueryAsync() > 0;    
        }

        public async Task<int> ContarAsync()
        {
            await using var cn = CrearConexion();
            await cn.OpenAsync();

            await using var cmd = new OracleCommand(
                "SELECT COUNT(*) FROM PRODUCTOS_L", cn);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }   

        private static void AgregarParametrosProducto(OracleCommand cmd, Producto p)
        {
            cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2, 100).Value = p.Nombre.Trim();
            cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2, 500).Value =
                p.Descripcion.Trim();
            cmd.Parameters.Add("p_precio", OracleDbType.Decimal).Value = p.Precio;
            cmd.Parameters.Add("p_stock", OracleDbType.Int32).Value = p.Stock;
            cmd.Parameters.Add("p_categoria", OracleDbType.Varchar2, 100).Value = p.Categoria.Trim();
        }

        private static Producto LeerProducto(OracleDataReader r) => new()
        {
            Id = Convert.ToInt32(r["ID"]),
            Nombre = r["NOMBRE"].ToString() ?? "",
            Descripcion = r["DESCRIPCION"].ToString() ?? "",
            Precio = Convert.ToDecimal(r["PRECIO"]),
            Stock = Convert.ToInt32(r["STOCK"]),
            Categoria = r["CATEGORIA"].ToString() ?? "",
            FechaCreacion = Convert.ToDateTime(r["FECHA_CREACION"])
        };
    }   
}
    
