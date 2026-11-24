using CapaDominio;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class RepositorioFactura
    {
        // ==========================================
        // FACTURAS DE VENTA (EMITIDAS)
        // ==========================================

        public void AgregarVenta(FacturaVenta factura)
        {
            using (var conn = Conexion.ObtenerConexion())
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    string sqlFactura = @"INSERT INTO FacturasVenta (NroFactura, Fecha, ClienteId, ProyectoId,
SubTotal, IVA, MontoTotal)
                                          VALUES (@NroFactura, @Fecha, @ClienteId, @ProyectoId,
@SubTotal, @IVA, @MontoTotal);
                                          SELECT SCOPE_IDENTITY();";

                    int facturaId;
                    using (var cmd = new SqlCommand(sqlFactura, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@NroFactura", factura.NroFactura);
                        cmd.Parameters.AddWithValue("@Fecha", factura.Fecha);
                        cmd.Parameters.AddWithValue("@ClienteId", factura.Cliente.Id);
                        cmd.Parameters.AddWithValue("@ProyectoId",
                            factura.Proyecto != null ? (object)factura.Proyecto.Id : DBNull.Value);
                        cmd.Parameters.AddWithValue("@SubTotal", factura.SubTotal);
                        cmd.Parameters.AddWithValue("@IVA", factura.IVA);
                        cmd.Parameters.AddWithValue("@MontoTotal", factura.MontoTotal);

                        conn.Open();
                        facturaId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    string sqlDetalle = @"INSERT INTO DetallesFacturaVenta (FacturaVentaId, Descripcion, Monto)
                                          VALUES (@FacturaVentaId, @Descripcion, @Monto);";
                    foreach (var det in factura.Detalles)
                    {
                        using (var cmdDet = new SqlCommand(sqlDetalle, conn, transaction))
                        {
                            cmdDet.Parameters.AddWithValue("@FacturaVentaId", facturaId);
                            cmdDet.Parameters.AddWithValue("@Descripcion", det.Descripcion);
                            cmdDet.Parameters.AddWithValue("@Monto", det.Monto);
                            cmdDet.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    factura.Id = facturaId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public List<FacturaVenta> ObtenerVentas()
        {
            return ConsultarVentas("SELECT Id, NroFactura, Fecha, ClienteId, ProyectoId, SubTotal, IVA, MontoTotal FROM FacturasVenta", null);
        }

        public List<FacturaVenta> ObtenerVentasPorMes(int mes, int anio)
        {
            return ConsultarVentas(@"SELECT Id, NroFactura, Fecha, ClienteId, ProyectoId, SubTotal, IVA, MontoTotal
                                    FROM FacturasVenta
                                    WHERE MONTH(Fecha) = @Mes AND YEAR(Fecha) = @Anio",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Mes", mes);
                    cmd.Parameters.AddWithValue("@Anio", anio);
                });
        }

        public List<FacturaVenta> ObtenerVentasPorCliente(int clienteId)
        {
            return ConsultarVentas(@"SELECT Id, NroFactura, Fecha, ClienteId, ProyectoId, SubTotal, IVA, MontoTotal
                                    FROM FacturasVenta WHERE ClienteId = @ClienteId",
                cmd => cmd.Parameters.AddWithValue("@ClienteId", clienteId));
        }

        public List<FacturaVenta> ObtenerVentasPorProyecto(int proyectoId)
        {
            return ConsultarVentas(@"SELECT Id, NroFactura, Fecha, ClienteId, ProyectoId, SubTotal, IVA, MontoTotal
                                    FROM FacturasVenta WHERE ProyectoId = @ProyectoId",
                cmd => cmd.Parameters.AddWithValue("@ProyectoId", proyectoId));
        }

        // ==========================================
        // FACTURAS DE COMPRA
        // ==========================================

        public void AgregarCompra(FacturaCompra factura)
        {
            const string sql = @"INSERT INTO FacturasCompra
                    (NroFactura, Fecha, ProveedorRazonSocial, ProveedorRUT, ProveedorDireccion, SubTotal, IVA, MontoTotal)
                VALUES
                    (@NroFactura, @Fecha, @ProveedorRazonSocial, @ProveedorRUT, @ProveedorDireccion, @SubTotal, @IVA, @MontoTotal);
                SELECT SCOPE_IDENTITY();";

            using (var conn = Conexion.ObtenerConexion())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@NroFactura", factura.NroFactura);
                cmd.Parameters.AddWithValue("@Fecha", factura.Fecha);
                cmd.Parameters.AddWithValue("@ProveedorRazonSocial", factura.ProveedorRazonSocial);
                cmd.Parameters.AddWithValue("@ProveedorRUT", factura.ProveedorRUT);
                cmd.Parameters.AddWithValue("@ProveedorDireccion", factura.ProveedorDireccion);
                cmd.Parameters.AddWithValue("@SubTotal", factura.SubTotal);
                cmd.Parameters.AddWithValue("@IVA", factura.IVA);
                cmd.Parameters.AddWithValue("@MontoTotal", factura.MontoTotal);

                conn.Open();
                factura.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public List<FacturaCompra> ObtenerCompras()
        {
            return ConsultarCompras(@"SELECT Id, NroFactura, Fecha, ProveedorRazonSocial, ProveedorRUT,
ProveedorDireccion, SubTotal, IVA, MontoTotal
                                    FROM FacturasCompra ORDER BY Fecha DESC", null);
        }

        public List<FacturaCompra> ObtenerComprasPorMes(int mes, int anio)
        {
            return ConsultarCompras(@"SELECT Id, NroFactura, Fecha, ProveedorRazonSocial, ProveedorRUT,
ProveedorDireccion, SubTotal, IVA, MontoTotal
                                    FROM FacturasCompra
                                    WHERE MONTH(Fecha) = @Mes AND YEAR(Fecha) = @Anio
                                    ORDER BY Fecha ASC",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Mes", mes);
                    cmd.Parameters.AddWithValue("@Anio", anio);
                });
        }

        // ==========================================
        // HELPERS PRIVADOS
        // ==========================================

        private List<FacturaVenta> ConsultarVentas(string sql, Action<SqlCommand>? configurarParametros)
        {
            var lista = new List<FacturaVenta>();
            using (var conn = Conexion.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    configurarParametros?.Invoke(cmd);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearFacturaVenta(reader));
                        }
                    }
                }

                foreach (var factura in lista)
                {
                    factura.Detalles = ObtenerDetallesVenta(conn, factura.Id);
                }
            }
            return lista;
        }

        private List<DetalleFacturaVenta> ObtenerDetallesVenta(SqlConnection conn, int facturaId)
        {
            var detalles = new List<DetalleFacturaVenta>();
            const string sql = "SELECT Id, FacturaVentaId, Descripcion, Monto FROM DetallesFacturaVenta WHERE FacturaVentaId = @FacturaVentaId";
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@FacturaVentaId", facturaId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        detalles.Add(new DetalleFacturaVenta
                        {
                            Id = reader.GetInt32(0),
                            FacturaVentaId = reader.GetInt32(1),
                            Descripcion = reader.GetString(2),
                            Monto = reader.GetDecimal(3)
                        });
                    }
                }
            }
            return detalles;
        }

        private static FacturaVenta MapearFacturaVenta(SqlDataReader reader)
        {
            return new FacturaVenta
            {
                Id = reader.GetInt32(0),
                NroFactura = reader.GetString(1),
                Fecha = reader.GetDateTime(2),
                Cliente = new Cliente { Id = reader.GetInt32(3) },
                Proyecto = reader.IsDBNull(4) ? null : new Proyecto { Id = reader.GetInt32(4) },
                SubTotal = reader.GetDecimal(5),
                IVA = reader.GetDecimal(6),
                MontoTotal = reader.GetDecimal(7)
            };
        }

        private List<FacturaCompra> ConsultarCompras(string sql, Action<SqlCommand>? configurarParametros)
        {
            var lista = new List<FacturaCompra>();
            using (var conn = Conexion.ObtenerConexion())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    configurarParametros?.Invoke(cmd);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new FacturaCompra
                            {
                                Id = reader.GetInt32(0),
                                NroFactura = reader.GetString(1),
                                Fecha = reader.GetDateTime(2),
                                ProveedorRazonSocial = reader.GetString(3),
                                ProveedorRUT = reader.GetString(4),
                                ProveedorDireccion = reader.GetString(5),
                                SubTotal = reader.GetDecimal(6),
                                IVA = reader.GetDecimal(7),
                                MontoTotal = reader.GetDecimal(8)
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}
