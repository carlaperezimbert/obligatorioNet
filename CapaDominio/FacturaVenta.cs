using System;
using System.Collections.Generic;

namespace CapaDominio
{
    public class FacturaVenta
    {
        public int Id { get; set; }
        public int NumeroFactura { get; set; }
        public DateTime Fecha { get; set; }
        public int ClienteId { get; set; }
        public int? ProyectoId { get; set; } 
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }

        public List<DetalleFacturaVenta> Detalles { get; set; } = new List<DetalleFacturaVenta>();
    }

    public class DetalleFacturaVenta
    {
        public int Id { get; set; }
        public int FacturaVentaId { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
    }
}
