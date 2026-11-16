using System;
using System.Collections.Generic;
using System.Linq;
using CapaDominio;
using CapaDatos;

namespace CapaLogica
{
    public class ServicioCliente
    {
        // Instanciamos los repositorios necesarios de CapaDatos (ajustar nombres si difieren en tu CapaDatos)
        private readonly RepositorioCliente _repoCliente = new RepositorioCliente();
        private readonly RepositorioProyecto _repoProyecto = new RepositorioProyecto();
        private readonly RepositorioFactura _repoFactura = new RepositorioFactura();

        public void AltaCliente(Cliente cliente)
        {
            ValidarDatosCliente(cliente);

            // Verificar que no exista otro cliente con el mismo RUT
            Cliente clienteExistente = _repoCliente.ObtenerPorRut(cliente.RUT);
            if (clienteExistente != null)
            {
                throw new Exception($"Ya existe un cliente registrado con el RUT {cliente.RUT}.");
            }

            _repoCliente.Agregar(cliente);
        }

        public void ModificarCliente(Cliente cliente)
        {
            if (cliente == null || cliente.Id <= 0)
            {
                throw new Exception("El ID del cliente no es válido para modificar.");
            }

            ValidarDatosCliente(cliente);

            // Verificar que el cliente realmente exista antes de actualizar
            Cliente actual = _repoCliente.ObtenerPorId(cliente.Id);
            if (actual == null)
            {
                throw new Exception("El cliente que intenta modificar no existe.");
            }

            // Verificar que si cambió el RUT, el nuevo no pertenezca a otro cliente distinto
            Cliente conMismoRut = _repoCliente.ObtenerPorRut(cliente.RUT);
            if (conMismoRut != null && conMismoRut.Id != cliente.Id)
            {
                throw new Exception($"El RUT {cliente.RUT} ya pertenece a otro cliente.");
            }

            _repoCliente.Modificar(cliente);
        }

        public void EliminarCliente(int idCliente)
        {
            if (idCliente <= 0)
            {
                throw new Exception("El ID del cliente no es válido.");
            }

            Cliente cliente = _repoCliente.ObtenerPorId(idCliente);
            if (cliente == null)
            {
                throw new Exception("El cliente que intenta eliminar no existe.");
            }

            // Regla de integridad: no borrar si tiene proyectos asociados
            var proyectos = _repoProyecto.ObtenerPorCliente(idCliente);
            if (proyectos != null && proyectos.Count > 0)
            {
                throw new Exception("No se puede eliminar el cliente porque tiene proyectos asociados.");
            }

            // Regla de integridad: no borrar si tiene facturas emitidas asociadas
            var facturas = _repoFactura.ObtenerVentasPorCliente(idCliente);
            if (facturas != null && facturas.Count > 0)
            {
                throw new Exception("No se puede eliminar el cliente porque tiene facturas registradas.");
            }

            _repoCliente.Eliminar(idCliente);
        }

        public List<Cliente> ListarClientes()
        {
            return _repoCliente.ObtenerTodos();
        }

        public Cliente ObtenerClientePorId(int idCliente)
        {
            if (idCliente <= 0)
            {
                throw new Exception("El ID del cliente no es válido.");
            }

            Cliente cliente = _repoCliente.ObtenerPorId(idCliente);
            if (cliente == null)
            {
                throw new Exception("No se encontró el cliente solicitado.");
            }

            return cliente;
        }

        // Método privado para centralizar las validaciones de los atributos del cliente
        private void ValidarDatosCliente(Cliente cliente)
        {
            if (cliente == null)
            {
                throw new Exception("Los datos del cliente no pueden ser nulos.");
            }

            if (string.IsNullOrWhiteSpace(cliente.RazonSocial))
            {
                throw new Exception("La Razón Social es obligatoria.");
            }

            if (string.IsNullOrWhiteSpace(cliente.RUT))
            {
                throw new Exception("El RUT es obligatorio.");
            }

            // Validación de formato RUT uruguayo (12 dígitos numéricos)
            string rutLimpio = cliente.RUT.Trim();
            if (rutLimpio.Length != 12 || !rutLimpio.All(char.IsDigit))
            {
                throw new Exception("El RUT debe contener exactamente 12 dígitos numéricos.");
            }

            if (string.IsNullOrWhiteSpace(cliente.Direccion))
            {
                throw new Exception("La dirección del cliente es obligatoria.");
            }
        }
    }
}