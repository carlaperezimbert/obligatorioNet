using CapaDominio;
using System.Data.SqlClient;


namespace CapaDatos
{
    public class RepositorioCliente
    {
        public int Agregar(Cliente cliente)
        {
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    string sql = @"INSERT INTO Clientes (RazonSocial, RUT, Direccion)
                           VALUES (@RazonSocial, @RUT, @Direccion);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@RazonSocial", cliente.RazonSocial);
                        cmd.Parameters.AddWithValue("@RUT", cliente.RUT);
                        cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion);

                        conn.Open();
                        object resultado = cmd.ExecuteScalar();

                        return resultado != null && resultado != DBNull.Value
                            ? Convert.ToInt32(resultado)
                            : 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[ERROR SQL] Error {ex.Number} al insertar cliente: {ex.Message}");
                throw; // O devolvé -1 si preferís manejar el fallo sin cortar el flujo arriba
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error inesperado al agregar cliente: {ex.Message}");
                throw;
            }
        }

        public bool Modificar(Cliente cliente)
        {
            using (var conn = Conexion.ObtenerConexion())
            {
                string sql = @"UPDATE Clientes
                               SET RazonSocial = @RazonSocial, RUT = @RUT, Direccion = @Direccion
                               WHERE Id = @Id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", cliente.Id);
                    cmd.Parameters.AddWithValue("@RazonSocial", cliente.RazonSocial);
                    cmd.Parameters.AddWithValue("@RUT", cliente.RUT);
                    cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Eliminar(int id)
        {
            using (var conn = Conexion.ObtenerConexion())
            {
                string sql = "DELETE FROM Clientes WHERE Id = @Id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public Cliente ObtenerPorId(int id)
        {
            using (var conn = Conexion.ObtenerConexion())
            {
                string sql = "SELECT Id, RazonSocial, RUT, Direccion FROM Clientes WHERE Id = @Id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearCliente(reader);
                        }
                    }
                }
            }
            return null;
        }

        public Cliente ObtenerPorRut(string rut)
        {
            using (var conn = Conexion.ObtenerConexion())
            {
                string sql = "SELECT Id, RazonSocial, RUT, Direccion FROM Clientes WHERE RUT = @RUT";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RUT", rut);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearCliente(reader);
                        }
                    }
                }
            }
            return null;
        }

        public List<Cliente> ObtenerTodos()
        {
            var lista = new List<Cliente>();

            using (var conn = Conexion.ObtenerConexion())
            {
                string sql = "SELECT Id, RazonSocial, RUT, Direccion FROM Clientes";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearCliente(reader));
                        }
                    }
                }
            }
            return lista;
        }

        private static Cliente MapearCliente(SqlDataReader reader)
        {
            return new Cliente
            {
                Id = reader.GetInt32(0),
                RazonSocial = reader.GetString(1),
                RUT = reader.GetString(2),
                Direccion = reader.GetString(3)
            };
        }
    }
}
