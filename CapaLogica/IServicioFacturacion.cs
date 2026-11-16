using CapaDominio;

namespace CapaLogica
{
    public interface IServicioFacturacion
    {
        List<FacturaCompra> ListarComprasPorMes(int mes, int anio);
        List<FacturaCompra> ListarFacturasCompras();
        List<FacturaVenta> ListarFacturasVentas();
        List<FacturaVenta> ListarVentasPorMes(int mes, int anio);
        void RegistrarFacturaCompra(FacturaCompra factura);
        void RegistrarFacturaVenta(FacturaVenta factura, bool llevaIva);
    }
}