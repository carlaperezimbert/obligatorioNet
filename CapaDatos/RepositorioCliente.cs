using CapaDominio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class RepositorioCliente : IClienteRepository
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
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
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
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Cliente
                        {
                            Id = reader.GetInt32(0),
                            RazonSocial = reader.GetString(1),
                            RUT = reader.GetString(2),
                            Direccion = reader.GetString(3)
                        });
                    }
                }
            }
            return lista;
        }
        
        //Falta cuerpo del metodo
        public Cliente ObtenerPorRut(string rUT)
        {
            throw new NotImplementedException();
        }
    }
}
