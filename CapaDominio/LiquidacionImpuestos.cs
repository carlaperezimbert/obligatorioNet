namespace CapaDominio
{
    public class LiquidacionImpuestos
    {
        public int Id { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public decimal TotalVentasSinIVA { get; set; }
        public decimal IVAVentas { get; set; }
        public decimal IVACompras { get; set; }
        public decimal SaldoIVA { get; set; } // IVA Ventas - IVA Compras
        public decimal IRAE { get; set; }      // 12% de la facturación
    }
}
