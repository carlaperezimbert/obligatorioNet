using CapaDominio;

namespace CapaLogica
{
    public interface IServicioImpuestos
    {
        LiquidacionImpuestos CalcularYGuardarLiquidacionMensual(int mes, int anio);
        LiquidacionImpuestos ConsultarLiquidacionPorMes(int mes, int anio);
    }
}