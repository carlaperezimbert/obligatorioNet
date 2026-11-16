using System;
using System.Collections.Generic;
using CapaDominio;
using CapaDatos;

namespace CapaLogica
{
    public class ServicioProyecto
    {
        private readonly RepositorioProyecto _repoProyecto = new RepositorioProyecto();
        private readonly RepositorioCliente _repoCliente = new RepositorioCliente();
        private readonly RepositorioFactura _repoFactura = new RepositorioFactura();

        public void AltaProyecto(Proyecto proyecto)
        {
            ValidarDatosProyecto(proyecto);

            // Verificar que el cliente asignado realmente exista en la base de datos
            Cliente clienteExistente = _repoCliente.ObtenerPorId(proyecto.Cliente.Id);
            if (clienteExistente == null)
            {
                throw new Exception("El cliente asociado al proyecto no existe.");
            }

            // Al crearse, todo proyecto arranca en estado Planificado y sin fecha de fin real
            proyecto.Estado = EstadoProyecto.Planificado;
            proyecto.FechaFin = null;

            _repoProyecto.Agregar(proyecto);
        }

        public void ModificarProyecto(Proyecto proyecto)
        {
            if (proyecto == null || proyecto.Id <= 0)
            {
                throw new Exception("El ID del proyecto no es válido para modificar.");
            }

            ValidarDatosProyecto(proyecto);

            Proyecto proyectoActual = _repoProyecto.ObtenerPorId(proyecto.Id);
            if (proyectoActual == null)
            {
                throw new Exception("El proyecto que intenta modificar no existe.");
            }

            // Verificar que el cliente asignado siga siendo válido
            Cliente clienteExistente = _repoCliente.ObtenerPorId(proyecto.Cliente.Id);
            if (clienteExistente == null)
            {
                throw new Exception("El cliente asociado al proyecto no existe.");
            }

            // Regla de negocio sobre estados y fechas de cierre
            if (proyectoActual.Estado == EstadoProyecto.Cancelado && proyecto.Estado == EstadoProyecto.Finalizado)
            {
                throw new Exception("No se puede finalizar un proyecto que ya fue cancelado.");
            }

            // Si pasa a Finalizado y no tiene fecha de fin cargada, se asigna la fecha actual
            if (proyecto.Estado == EstadoProyecto.Finalizado && proyecto.FechaFin == null)
            {
                proyecto.FechaFin = DateTime.Now;
            }
            // Si vuelve a un estado abierto (Planificado o EnCurso), limpiamos la fecha de fin real
            else if (proyecto.Estado == EstadoProyecto.Planificado || proyecto.Estado == EstadoProyecto.EnCurso)
            {
                proyecto.FechaFin = null;
            }

            _repoProyecto.Modificar(proyecto);
        }

        public void EliminarProyecto(int idProyecto)
        {
            if (idProyecto <= 0)
            {
                throw new Exception("El ID del proyecto no es válido.");
            }

            Proyecto proyecto = _repoProyecto.ObtenerPorId(idProyecto);
            if (proyecto == null)
            {
                throw new Exception("El proyecto que intenta eliminar no existe.");
            }

            // Regla de integridad: no permitir eliminar si ya tiene facturas emitidas vinculadas
            var facturasAsociadas = _repoFactura.ObtenerVentasPorProyecto(idProyecto);
            if (facturasAsociadas != null && facturasAsociadas.Count > 0)
            {
                throw new Exception("No se puede eliminar el proyecto porque ya tiene facturas emitidas asociadas.");
            }

            _repoProyecto.Eliminar(idProyecto);
        }

        public List<Proyecto> ListarProyectos()
        {
            return _repoProyecto.ObtenerTodos();
        }

        public List<Proyecto> ListarProyectosPorCliente(int idCliente)
        {
            if (idCliente <= 0)
            {
                throw new Exception("El ID del cliente no es válido.");
            }

            return _repoProyecto.ObtenerPorCliente(idCliente);
        }

        public Proyecto ObtenerProyectoPorId(int idProyecto)
        {
            if (idProyecto <= 0)
            {
                throw new Exception("El ID del proyecto no es válido.");
            }

            Proyecto proyecto = _repoProyecto.ObtenerPorId(idProyecto);
            if (proyecto == null)
            {
                throw new Exception("No se encontró el proyecto solicitado.");
            }

            return proyecto;
        }

        // Validaciones centrales del proyecto
        private void ValidarDatosProyecto(Proyecto proyecto)
        {
            if (proyecto == null)
            {
                throw new Exception("Los datos del proyecto no pueden ser nulos.");
            }

            if (proyecto.Cliente == null || proyecto.Cliente.Id <= 0)
            {
                throw new Exception("Debe asociar un cliente válido al proyecto.");
            }

            if (string.IsNullOrWhiteSpace(proyecto.Nombre))
            {
                throw new Exception("El nombre del proyecto es obligatorio.");
            }

            if (proyecto.PresupuestoInicial <= 0)
            {
                throw new Exception("El presupuesto inicial debe ser mayor a cero.");
            }

            if (proyecto.FechaInicio == DateTime.MinValue)
            {
                throw new Exception("La fecha de inicio no es válida.");
            }

            if (proyecto.FechaFinPlanificada < proyecto.FechaInicio)
            {
                throw new Exception("La fecha de fin planificada no puede ser anterior a la fecha de inicio.");
            }

            if (proyecto.FechaFin.HasValue && proyecto.FechaFin.Value < proyecto.FechaInicio)
            {
                throw new Exception("La fecha de fin real no puede ser anterior a la fecha de inicio.");
            }
        }
    }
}