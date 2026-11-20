using System;

namespace CapaDominio
{
    public class LiquidacionImpuestos
    {
        public int Id { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public DateTime FechaCalculo { get; set; }
        public decimal TotalFacturado { get; set; }
        public decimal TotalIvaVentas { get; set; }
        public decimal TotalIvaCompras { get; set; }
        public decimal MontoIva { get; set; }   // IVA Ventas - IVA Compras
        public decimal MontoIrae { get; set; }  // 12% de la facturación
    }
}
