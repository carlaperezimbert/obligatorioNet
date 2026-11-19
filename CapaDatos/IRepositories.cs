using CapaDominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace CapaDatos
{
    public interface IClienteRepository
    {
        int Agregar(Cliente cliente);
        bool Modificar(Cliente cliente);
        bool Eliminar(int id);
        Cliente ObtenerPorId(int id);
        List<Cliente> ObtenerTodos();
    }
    public interface IProyectoRepository
    {
        int Agregar(Proyecto proyecto);
        bool Modificar(Proyecto proyecto);
        bool Eliminar(int id);
        Proyecto ObtenerPorId(int id);
        List<Proyecto> ObtenerTodos();
    }
    public interface IFacturaEmitidaRepository
    {
        int RegistrarFactura(FacturaEmitida factura);
        List<FacturaEmitida> ObtenerPorMes(int anio, int mes);
    }
    public interface IFacturaCompraRepository
    {
        int RegistrarCompra(FacturaCompra compra);
        List<FacturaCompra> ObtenerPorMes(int anio, int mes);
    }
    public interface IImpuestoRepository
    {
        LiquidacionFiscalDTO ObtenerLiquidacionMensual(int anio, int mes);
    }
}
}
