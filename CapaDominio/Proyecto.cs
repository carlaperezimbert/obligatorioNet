using System;

namespace CapaDominio
{
    public class Proyecto
    {
        public int Id { get; set; }
        public Cliente Cliente { get; set; }
        public string Nombre { get; set; }
        public TipoProyecto Tipo { get; set; }
        public DateTime FechaInicio { get; set; }
        public decimal PresupuestoInicial { get; set; }
        public DateTime FechaFinPlanificada { get; set; }
        public DateTime? FechaFin { get; set; } // Nullable porque puede estar en curso
        public EstadoProyecto Estado { get; set; }
    }
}
