using CapaDominio;

namespace CapaDatos
{
    public class RepositorioImpuestos
    {
        public LiquidacionImpuestos ObtenerPorMes(int mes, int anio)
        {
            using (var conn = Conexion.ObtenerConexion())
            {
                const string sql = @"SELECT Id, Mes, Anio, FechaCalculo, TotalFacturado, TotalIvaVentas,
TotalIvaCompras, MontoIva, MontoIrae
                                    FROM LiquidacionesImpuestos
                                    WHERE Mes = @Mes AND Anio = @Anio";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Mes", mes);
                    cmd.Parameters.AddWithValue("@Anio", anio);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearLiquidacion(reader);
                        }
                    }
                }
            }
            return null;
        }

        public void GuardarLiquidacion(LiquidacionImpuestos liquidacion)
        {
            const string sql = @"INSERT INTO LiquidacionesImpuestos
                    (Mes, Anio, FechaCalculo, TotalFacturado, TotalIvaVentas, TotalIvaCompras, MontoIva, MontoIrae)
                VALUES
                    (@Mes, @Anio, @FechaCalculo, @TotalFacturado, @TotalIvaVentas, @TotalIvaCompras, @MontoIva, @MontoIrae);
                SELECT SCOPE_IDENTITY();";

            using (var conn = Conexion.ObtenerConexion())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Mes", liquidacion.Mes);
                cmd.Parameters.AddWithValue("@Anio", liquidacion.Anio);
                cmd.Parameters.AddWithValue("@FechaCalculo", liquidacion.FechaCalculo);
                cmd.Parameters.AddWithValue("@TotalFacturado", liquidacion.TotalFacturado);
                cmd.Parameters.AddWithValue("@TotalIvaVentas", liquidacion.TotalIvaVentas);
                cmd.Parameters.AddWithValue("@TotalIvaCompras", liquidacion.TotalIvaCompras);
                cmd.Parameters.AddWithValue("@MontoIva", liquidacion.MontoIva);
                cmd.Parameters.AddWithValue("@MontoIrae", liquidacion.MontoIrae);

                conn.Open();
                liquidacion.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private static LiquidacionImpuestos MapearLiquidacion(SqlDataReader reader)
        {
            return new LiquidacionImpuestos
            {
                Id = reader.GetInt32(0),
                Mes = reader.GetInt32(1),
                Anio = reader.GetInt32(2),
                FechaCalculo = reader.GetDateTime(3),
                TotalFacturado = reader.GetDecimal(4),
                TotalIvaVentas = reader.GetDecimal(5),
                TotalIvaCompras = reader.GetDecimal(6),
                MontoIva = reader.GetDecimal(7),
                MontoIrae = reader.GetDecimal(8)
            };
        }
    }
}
