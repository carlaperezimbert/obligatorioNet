using System;
using System.Collections.Generic;
using System.Linq;
using CapaDominio;
using CapaDatos;

namespace CapaLogica
{
    public class ServicioFacturacion : IServicioFacturacion
    {
        private readonly RepositorioFactura _repoFactura = new RepositorioFactura();
        private readonly RepositorioFactura _repoFactura = new RepositorioFactura();
        private readonly RepositorioCliente _repoCliente = new RepositorioCliente();
        private readonly RepositorioProyecto _repoProyecto = new RepositorioProyecto();

        // Tasa estándar de IVA en Uruguay (22%)
        private const decimal TASA_IVA = 0.22m;

        // ==========================================
        // 1. FACTURAS DE VENTA (EMITIDAS)
        // ==========================================

        public void RegistrarFacturaVenta(FacturaVenta facturaVenta, bool llevaIva)
        {
            ValidarDatosFacturaVenta(facturaVenta);

            // 1. Verificar que el cliente exista
            Cliente clienteExistente = _repoCliente.ObtenerPorId(facturaVenta.ClienteId);
            if (clienteExistente == null)
            {
                throw new Exception("El cliente seleccionado para la factura no existe.");
            }

            // 2. Si viene un proyecto asociado (es opcional según la letra), validar que exista y sea de ese cliente
            if (facturaVenta.ProyectoId != null && facturaVenta.ProyectoId > 0)
            {
                Proyecto proyectoExistente = _repoProyecto.ObtenerPorId(facturaVenta.ProyectoId);
                if (proyectoExistente == null)
                {
                    throw new Exception("El proyecto asociado a la factura no existe.");
                }

                if (proyectoExistente.ClienteId != facturaVenta.ClienteId)
                {
                    throw new Exception("El proyecto seleccionado no pertenece al cliente de la factura.");
                }

                if (proyectoExistente.Estado == EstadoProyecto.Cancelado)
                {
                    throw new Exception("No se puede facturar un proyecto que se encuentra cancelado.");
                }
            }

            // 3. Cálculos automáticos en la capa lógica (nunca confiar solo en la interfaz)
            facturaVenta.Subtotal = facturaVenta.Detalles.Sum(d => d.Monto);
            facturaVenta.IVA = llevaIva ? Math.Round(facturaVenta.Subtotal * TASA_IVA, 2) : 0m;
            facturaVenta.Total = facturaVenta.Subtotal + facturaVenta.IVA;

            // Si no trae fecha asignada, se registra con la fecha actual
            if (facturaVenta.Fecha == DateTime.MinValue)
            {
                facturaVenta.Fecha = DateTime.Now;
            }

            _repoFactura.AgregarVenta(facturaVenta);
        }

        public List<FacturaVenta> ListarFacturasVentas()
        {
            return _repoFactura.ObtenerVentas();
        }

        public List<FacturaVenta> ListarVentasPorMes(int mes, int anio)
        {
            ValidarMesYAnio(mes, anio);
            return _repoFactura.ObtenerVentasPorMes(mes, anio);
        }

        // ==========================================
        // 2. FACTURAS DE COMPRA
        // ==========================================

        public void RegistrarFacturaCompra(FacturaCompra facturaCompra)
        {
            ValidarDatosFacturaCompra(facturaCompra);

            // Aseguramos coherencia en el monto total (SubTotal + IVA)
            facturaCompra.Total = facturaCompra.Subtotal + facturaCompra.IVA;

            if (facturaCompra.Fecha == DateTime.MinValue)
            {
                facturaCompra.Fecha = DateTime.Now;
            }

            _repoFactura.AgregarCompra(facturaCompra);
        }

        public List<FacturaCompra> ListarFacturasCompras()
        {
            return _repoFactura.ObtenerCompras();
        }

        public List<FacturaCompra> ListarComprasPorMes(int mes, int anio)
        {
            ValidarMesYAnio(mes, anio);
            return _repoFactura.ObtenerComprasPorMes(mes, anio);
        }

        // ==========================================
        // 3. VALIDACIONES PRIVADAS
        // ==========================================

        private void ValidarDatosFacturaVenta(FacturaVenta facturaVenta)
        {
            if (facturaVenta == null)
            {
                throw new Exception("Los datos de la factura de venta no pueden ser nulos.");
            }

            //if (int.IsNull(facturaVenta.NumeroFactura))
            //{
            //    throw new Exception("El número de factura es obligatorio.");
            //}

            if (facturaVenta.ClienteId == null || facturaVenta.ClienteId <= 0)
            {
                throw new Exception("Debe indicar un cliente válido para emitir la factura.");
            }

            if (facturaVenta.Detalles == null || facturaVenta.Detalles.Count == 0)
            {
                throw new Exception("La factura debe contener al menos un renglón de detalle.");
            }

            foreach (var renglon in facturaVenta.Detalles)
            {
                if (string.IsNullOrWhiteSpace(renglon.Descripcion))
                {
                    throw new Exception("Todos los renglones de la factura deben tener una descripción.");
                }

                if (renglon.Monto <= 0)
                {
                    throw new Exception($"El monto del renglón '{renglon.Descripcion}' debe ser mayor a cero.");
                }
            }
        }

        private void ValidarDatosFacturaCompra(FacturaCompra facturaCompra)
        {
            if (facturaCompra == null)
            {
                throw new Exception("Los datos de la factura de compra no pueden ser nulos.");
            }

            //if (string.IsNullOrWhiteSpace(facturaCompra.NumeroFactura))
            //{
            //    throw new Exception("El número de factura de compra es obligatorio.");
            //}

            // Validación de los datos del proveedor exigidos por la letra
            if (string.IsNullOrWhiteSpace(facturaCompra.ProveedorRazonSocial))
            {
                throw new Exception("La Razón Social del proveedor es obligatoria.");
            }

            if (string.IsNullOrWhiteSpace(facturaCompra.ProveedorRUT))
            {
                throw new Exception("El RUT del proveedor es obligatorio.");
            }

            string rutLimpio = facturaCompra.ProveedorRUT.Trim();
            if (rutLimpio.Length != 12 || !rutLimpio.All(char.IsDigit))
            {
                throw new Exception("El RUT del proveedor debe contener exactamente 12 dígitos numéricos.");
            }

            if (string.IsNullOrWhiteSpace(facturaCompra.ProveedorDireccion))
            {
                throw new Exception("La dirección del proveedor es obligatoria.");
            }

            if (facturaCompra.Subtotal <= 0)
            {
                throw new Exception("El subtotal de la factura de compra debe ser mayor a cero.");
            }

            if (facturaCompra.IVA < 0)
            {
                throw new Exception("El IVA de la factura de compra no puede ser negativo.");
            }
        }

        private void ValidarMesYAnio(int mes, int anio)
        {
            if (mes < 1 || mes > 12)
            {
                throw new Exception("El mes debe estar comprendido entre 1 y 12");
            }
        }
    }
}