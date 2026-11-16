using System;
using System.Collections.Generic;
using System.Linq;
using CapaDominio;
using CapaDatos;

namespace CapaLogica
{
    public class ServicioImpuestos
    {
        private readonly RepositorioImpuestos _repoImpuestos = new RepositorioImpuestos();
        private readonly RepositorioFactura _repoFactura = new RepositorioFactura();

        // Porcentaje de IRAE establecido en la letra del obligatorio (12% de la facturación)
        private const decimal TASA_IRAE = 0.12m;

        public LiquidacionImpuestos CalcularYGuardarLiquidacionMensual(int mes, int anio)
        {
            ValidarMesYAnio(mes, anio);

            // 1. Verificar si ya existe una liquidación cerrada para ese mes y año
            LiquidacionImpuestos existente = _repoImpuestos.ObtenerPorMes(mes, anio);
            if (existente != null)
            {
                throw new Exception($"Ya existe una liquidación de impuestos registrada para el período {mes:D2}/{anio}. Puede consultarla en el historial.");
            }

            // 2. Obtener todas las facturas de venta y de compra del período seleccionado
            List<FacturaVenta> ventasDelMes = _repoFactura.ObtenerVentasPorMes(mes, anio) ?? new List<FacturaVenta>();
            List<FacturaCompra> comprasDelMes = _repoFactura.ObtenerComprasPorMes(mes, anio) ?? new List<FacturaCompra>();

            // 3. Cálculo de totales de IVA (Ventas - Compras)
            decimal totalIvaVentas = ventasDelMes.Sum(v => v.IVA);
            decimal totalIvaCompras = comprasDelMes.Sum(c => c.IVA);
            decimal saldoIva = Math.Round(totalIvaVentas - totalIvaCompras, 2);

            // 4. Cálculo de IRAE (12% sobre la facturación neta / SubTotal de ventas del mes)
            decimal totalFacturacion = ventasDelMes.Sum(v => v.SubTotal);
            decimal montoIrae = Math.Round(totalFacturacion * TASA_IRAE, 2);

            // 5. Armar el objeto de dominio con el detalle del cálculo
            LiquidacionImpuestos nuevaLiquidacion = new LiquidacionImpuestos
            {
                Mes = mes,
                Anio = anio,
                FechaCalculo = DateTime.Now,
                TotalFacturado = totalFacturacion,
                TotalIvaVentas = totalIvaVentas,
                TotalIvaCompras = totalIvaCompras,
                MontoIva = saldoIva,
                MontoIrae = montoIrae
            };

            // 6. Guardar en la base de datos para futuras consultas
            _repoImpuestos.GuardarLiquidacion(nuevaLiquidacion);

            return nuevaLiquidacion;
        }

        public LiquidacionImpuestos ConsultarLiquidacionPorMes(int mes, int anio)
        {
            ValidarMesYAnio(mes, anio);

            LiquidacionImpuestos liquidacion = _repoImpuestos.ObtenerPorMes(mes, anio);
            if (liquidacion == null)
            {
                throw new Exception($"No se encontró ninguna liquidación calculada para el período {mes:D2}/{anio}.");
            }

            return liquidacion;
        }

        private void ValidarMesYAnio(int mes, int anio)
        {
            if (mes < 1 || mes > 12)
            {
                throw new Exception("El mes seleccionado debe estar entre 1 y 12.");
            }

            if (anio < 2000 || anio > DateTime.Now.Year + 1)
            {
                throw new Exception("El año ingresado no es válido.");
            }
        }
    }
}