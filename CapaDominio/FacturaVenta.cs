using System;
using System.Collections.Generic;

namespace CapaDominio
{
    public class FacturaVenta
    {
        public int Id { get; set; }
        public string NroFactura { get; set; }
        public DateTime Fecha { get; set; }
        public Cliente Cliente { get; set; }
        public Proyecto? Proyecto { get; set; }
        public List<DetalleFacturaVenta> Detalles { get; set; } = new List<DetalleFacturaVenta>();
        public decimal SubTotal { get; set; }
        public decimal IVA { get; set; }
        public decimal MontoTotal { get; set; }
    }

    public class DetalleFacturaVenta
    {
        public int Id { get; set; }
        public int FacturaVentaId { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
    }
}
