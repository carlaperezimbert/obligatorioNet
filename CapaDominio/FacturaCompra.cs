using System;

namespace CapaDominio
{
    public class FacturaCompra
    {
        public int Id { get; set; }
        public int NumeroFactura { get; set; }
        public DateTime Fecha { get; set; }
        public string ProveedorRazonSocial { get; set; }
        public string ProveedorRUT { get; set; }
        public string ProveedorDireccion { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
    }
}
