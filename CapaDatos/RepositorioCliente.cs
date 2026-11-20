using CapaDominio;

namespace CapaDatos
{
    public class RepositorioCliente
    {
        public int Agregar(Cliente cliente)
        {
            using (var conn = Conexion.ObtenerConexion())
            {
                string sql = @"INSERT INTO Clientes (RazonSocial, RUT, Direccion)
                               VALUES (@RazonSocial, @RUT, @Direccion);
                               SELECT SCOPE_IDENTITY();";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RazonSocial", cliente.RazonSocial);
                    cmd.Parameters.AddWithValue("@RUT", cliente.RUT);
                    cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion);

                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
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
