using CapaDominio;

namespace CapaLogica
{
    public interface IServicioProyecto
    {
        void AltaProyecto(Proyecto proyecto);
        void EliminarProyecto(int idProyecto);
        List<Proyecto> ListarProyectos();
        List<Proyecto> ListarProyectosPorCliente(int idCliente);
        void ModificarProyecto(Proyecto proyecto);
        Proyecto ObtenerProyectoPorId(int idProyecto);
    }
}