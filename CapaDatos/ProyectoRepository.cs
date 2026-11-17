using CapaDominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class ProyectoRepository : IProyectoRepository
    {
        public int Agregar(Proyecto proyecto)
        {
            using (var conn = Conexion.ObtenerConexion())
            {
                string sql = @"INSERT INTO Proyectos (ClienteId, Nombre, Tipo, FechaInicio, PresupuestoInicial,
FechaFinPlanificada, FechaFin, Estado)
                               VALUES (@ClienteId, @Nombre, @Tipo, @FechaInicio, @PresupuestoInicial,
@FechaFinPlanificada, @FechaFin, @Estado);
                               SELECT SCOPE_IDENTITY();";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ClienteId", proyecto.ClienteId);
                    cmd.Parameters.AddWithValue("@Nombre", proyecto.Nombre);
                    cmd.Parameters.AddWithValue("@Tipo", (int)proyecto.Tipo);
                    cmd.Parameters.AddWithValue("@FechaInicio", proyecto.FechaInicio);
                    cmd.Parameters.AddWithValue("@PresupuestoInicial", proyecto.PresupuestoInicial);
                    cmd.Parameters.AddWithValue("@FechaFinPlanificada", proyecto.FechaFinPlanificada);
                    cmd.Parameters.AddWithValue("@FechaFin", (object)proyecto.FechaFin ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (int)proyecto.Estado);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public bool Modificar(Proyecto proyecto)
        {
            using (var conn = Conexion.ObtenerConexion())
            {
                string sql = @"UPDATE Proyectos
                               SET ClienteId=@ClienteId, Nombre=@Nombre, Tipo=@Tipo, FechaInicio=@FechaInicio,
                                   PresupuestoInicial=@PresupuestoInicial, FechaFinPlanificada=@FechaFinPlanificada,
                                   FechaFin=@FechaFin, Estado=@Estado
                               WHERE Id = @Id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", proyecto.Id);
                    cmd.Parameters.AddWithValue("@ClienteId", proyecto.ClienteId);
                    cmd.Parameters.AddWithValue("@Nombre", proyecto.Nombre);
                    cmd.Parameters.AddWithValue("@Tipo", (int)proyecto.Tipo);
                    cmd.Parameters.AddWithValue("@FechaInicio", proyecto.FechaInicio);
                    cmd.Parameters.AddWithValue("@PresupuestoInicial", proyecto.PresupuestoInicial);
                    cmd.Parameters.AddWithValue("@FechaFinPlanificada", proyecto.FechaFinPlanificada);
                    cmd.Parameters.AddWithValue("@FechaFin", (object)proyecto.FechaFin ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (int)proyecto.Estado);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public bool Eliminar(int id)
        {
            using (var conn = Conexion.ObtenerConexion())
            {
                string sql = "DELETE FROM Proyectos WHERE Id = @Id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public Proyecto ObtenerPorId(int id)
        {
            using (var conn = Conexion.ObtenerConexion())
            {
                string sql = "SELECT Id, ClienteId, Nombre, Tipo, FechaInicio, PresupuestoInicial,
FechaFinPlanificada, FechaFin, Estado FROM Proyectos WHERE Id = @Id";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapearProyecto(reader);
                    }
                }
            }
            return null;
        }
        public List<Proyecto> ObtenerTodos()
        {
            var lista = new List<Proyecto>();
            using (var conn = Conexion.ObtenerConexion())
            {
                string sql = "SELECT Id, ClienteId, Nombre, Tipo, FechaInicio, PresupuestoInicial,
FechaFinPlanificada, FechaFin, Estado FROM Proyectos";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(MapearProyecto(reader));
                    }
                }
            }
            return lista;
        }
        private Proyecto MapearProyecto(SqlDataReader reader)
        {
            return new Proyecto
            {
                Id = reader.GetInt32(0),
                ClienteId = reader.GetInt32(1),
                Nombre = reader.GetString(2),
                Tipo = (TipoProyecto)reader.GetInt32(3),
                FechaInicio = reader.GetDateTime(4),
                PresupuestoInicial = Convert.ToDouble(reader.GetDecimal(5)),
                FechaFinPlanificada = reader.GetDateTime(6),
                FechaFin = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                Estado = (EstadoProyecto)reader.GetInt32(8)
            };
        }
    }
}