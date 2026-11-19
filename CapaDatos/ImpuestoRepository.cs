using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class ImpuestoRepository : IImpuestoRepository
    {
        public LiquidacionFiscalDTO ObtenerLiquidacionMensual(int anio, int mes)
        {
            using (var conn = Conexion.ObtenerConexion())
            {
                using (var cmd = new SqlCommand("dbo.sp_CalcularLiquidacionFiscalMensual", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Anio", anio);
                    cmd.Parameters.AddWithValue("@Mes", mes);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LiquidacionFiscalDTO
                            {
                                Anio = reader.GetInt32(reader.GetOrdinal("Anio")),
                                Mes = reader.GetInt32(reader.GetOrdinal("Mes")),
                                FacturacionTotal =
Convert.ToDouble(reader.GetDecimal(reader.GetOrdinal("FacturacionTotal"))),
                                TotalIvaVentas =
Convert.ToDouble(reader.GetDecimal(reader.GetOrdinal("TotalIvaVentas"))),
                                TotalIvaCompras =
Convert.ToDouble(reader.GetDecimal(reader.GetOrdinal("TotalIvaCompras"))),
                                LiquidacionIva =
Convert.ToDouble(reader.GetDecimal(reader.GetOrdinal("LiquidacionIva"))),
                                LiquidacionIrae =
Convert.ToDouble(reader.GetDecimal(reader.GetOrdinal("LiquidacionIrae")))
                            };
                        }
                    }
                }
            }
            return new LiquidacionFiscalDTO { Anio = anio, Mes = mes };
        }
    }
}
