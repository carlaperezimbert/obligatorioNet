using System;
using System.Collections.Generic;
using System.Globalization;
using CapaDominio;
using CapaLogica;

namespace AppConsola
{
    internal class Program
    {
        private static readonly IServicioCliente _servicioCliente = new ServicioCliente();
        private static readonly IServicioProyecto _servicioProyecto = new ServicioProyecto();
        private static readonly IServicioFacturacion _servicioFacturacion = new ServicioFacturacion();
        private static readonly IServicioImpuestos _servicioImpuestos = new ServicioImpuestos();

        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("  SISTEMA DE GESTION - OBLIGATORIO .NET  ");
            Console.WriteLine("========================================");

            bool salir = false;
            while (!salir)
            {
                MostrarMenuPrincipal();
                string opcion = Console.ReadLine()?.Trim() ?? string.Empty;

                try
                {
                    switch (opcion)
                    {
                        case "1": MenuClientes(); break;
                        case "2": MenuProyectos(); break;
                        case "3": MenuFacturasVenta(); break;
                        case "4": MenuFacturasCompra(); break;
                        case "5": MenuImpuestos(); break;
                        case "0":
                            salir = true;
                            Console.WriteLine("\nSaliendo del sistema...");
                            break;
                        default:
                            Console.WriteLine("Opción no válida.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERROR] {ex.Message}");
                }

                if (!salir)
                {
                    Pausa();
                }
            }
        }

        // ==========================================
        // MENÚ PRINCIPAL
        // ==========================================

        private static void MostrarMenuPrincipal()
        {
            Console.WriteLine();
            Console.WriteLine("----- MENÚ PRINCIPAL -----");
            Console.WriteLine("1. Gestión de Clientes");
            Console.WriteLine("2. Gestión de Proyectos");
            Console.WriteLine("3. Facturas de Venta");
            Console.WriteLine("4. Facturas de Compra");
            Console.WriteLine("5. Impuestos (Liquidación mensual)");
            Console.WriteLine("0. Salir");
            Console.Write("\nSeleccione una opción: ");
        }

        // ==========================================
        // CLIENTES
        // ==========================================

        private static void MenuClientes()
        {
            bool volver = false;
            while (!volver)
            {
                Console.WriteLine();
                Console.WriteLine("----- CLIENTES -----");
                Console.WriteLine("1. Alta de cliente");
                Console.WriteLine("2. Modificar cliente");
                Console.WriteLine("3. Eliminar cliente");
                Console.WriteLine("4. Listar clientes");
                Console.WriteLine("5. Ver cliente por Id");
                Console.WriteLine("0. Volver");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": AltaCliente(); break;
                    case "2": ModificarCliente(); break;
                    case "3": EliminarCliente(); break;
                    case "4": ListarClientes(); break;
                    case "5": VerCliente(); break;
                    case "0": volver = true; break;
                    default: Console.WriteLine("Opción no válida."); break;
                }
            }
        }

        private static void AltaCliente()
        {
            Console.WriteLine("\n--- Alta de cliente ---");
            Cliente cliente = new Cliente
            {
                RazonSocial = LeerTexto("Razón Social: ", obligatorio: true),
                RUT = LeerTexto("RUT (12 dígitos): ", obligatorio: true),
                Direccion = LeerTexto("Dirección: ", obligatorio: true)
            };

            _servicioCliente.AltaCliente(cliente);
            Console.WriteLine("[OK] Cliente registrado correctamente.");
        }

        private static void ModificarCliente()
        {
            Console.WriteLine("\n--- Modificar cliente ---");
            int id = LeerEntero("Id del cliente: ");
            Cliente cliente = _servicioCliente.ObtenerClientePorId(id);

            Console.WriteLine($"Datos actuales -> Razón Social: {cliente.RazonSocial} | RUT: {cliente.RUT} | Dirección: {cliente.Direccion}");
            Console.WriteLine("Presione Enter para mantener el valor actual.");

            cliente.RazonSocial = LeerTexto("Nueva Razón Social: ", obligatorio: false, actual: cliente.RazonSocial);
            cliente.RUT = LeerTexto("Nuevo RUT (12 dígitos): ", obligatorio: false, actual: cliente.RUT);
            cliente.Direccion = LeerTexto("Nueva Dirección: ", obligatorio: false, actual: cliente.Direccion);

            _servicioCliente.ModificarCliente(cliente);
            Console.WriteLine("[OK] Cliente modificado correctamente.");
        }

        private static void EliminarCliente()
        {
            Console.WriteLine("\n--- Eliminar cliente ---");
            int id = LeerEntero("Id del cliente a eliminar: ");
            Cliente cliente = _servicioCliente.ObtenerClientePorId(id);

            Console.WriteLine($"Se eliminará: {cliente.RazonSocial} (RUT {cliente.RUT}). ¿Confirma? (S/N): ");
            if (string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase))
            {
                _servicioCliente.EliminarCliente(id);
                Console.WriteLine("[OK] Cliente eliminado.");
            }
            else
            {
                Console.WriteLine("Operación cancelada.");
            }
        }

        private static void ListarClientes()
        {
            Console.WriteLine("\n--- Listado de clientes ---");
            List<Cliente> clientes = _servicioCliente.ListarClientes();

            if (clientes.Count == 0)
            {
                Console.WriteLine("No hay clientes registrados.");
                return;
            }

            Console.WriteLine($"{"Id",-5} {"RUT",-15} {"Razón Social",-40} Dirección");
            Console.WriteLine(new string('-', 100));
            foreach (var c in clientes)
            {
                Console.WriteLine($"{c.Id,-5} {c.RUT,-15} {c.RazonSocial,-40} {c.Direccion}");
            }
        }

        private static void VerCliente()
        {
            int id = LeerEntero("Id del cliente: ");
            Cliente cliente = _servicioCliente.ObtenerClientePorId(id);
            Console.WriteLine($"\nId: {cliente.Id}");
            Console.WriteLine($"Razón Social: {cliente.RazonSocial}");
            Console.WriteLine($"RUT: {cliente.RUT}");
            Console.WriteLine($"Dirección: {cliente.Direccion}");
        }

        // ==========================================
        // PROYECTOS
        // ==========================================

        private static void MenuProyectos()
        {
            bool volver = false;
            while (!volver)
            {
                Console.WriteLine();
                Console.WriteLine("----- PROYECTOS -----");
                Console.WriteLine("1. Alta de proyecto");
                Console.WriteLine("2. Modificar proyecto");
                Console.WriteLine("3. Eliminar proyecto");
                Console.WriteLine("4. Listar proyectos");
                Console.WriteLine("5. Proyectos por cliente");
                Console.WriteLine("6. Ver proyecto por Id");
                Console.WriteLine("0. Volver");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": AltaProyecto(); break;
                    case "2": ModificarProyecto(); break;
                    case "3": EliminarProyecto(); break;
                    case "4": ListarProyectos(_servicioProyecto.ListarProyectos(), "Todos los proyectos"); break;
                    case "5":
                        int idCliente = LeerEntero("Id del cliente: ");
                        ListarProyectos(_servicioProyecto.ListarProyectosPorCliente(idCliente), $"Proyectos del cliente {idCliente}");
                        break;
                    case "6": VerProyecto(); break;
                    case "0": volver = true; break;
                    default: Console.WriteLine("Opción no válida."); break;
                }
            }
        }

        private static void AltaProyecto()
        {
            Console.WriteLine("\n--- Alta de proyecto ---");
            int clienteId = LeerEntero("Id del cliente: ");
            Cliente cliente = _servicioCliente.ObtenerClientePorId(clienteId); // valida que exista

            Proyecto proyecto = LeerDatosProyecto(new Proyecto { Cliente = cliente });
            proyecto.FechaFin = null;

            _servicioProyecto.AltaProyecto(proyecto);
            Console.WriteLine("[OK] Proyecto registrado correctamente.");
        }

        private static void ModificarProyecto()
        {
            Console.WriteLine("\n--- Modificar proyecto ---");
            int id = LeerEntero("Id del proyecto: ");
            Proyecto proyecto = _servicioProyecto.ObtenerProyectoPorId(id);

            Console.WriteLine($"Datos actuales -> Nombre: {proyecto.Nombre} | ClienteId: {proyecto.Cliente.Id} | Estado: {proyecto.Estado}");

            int nuevoClienteId = LeerEnteroConActual("Id del cliente (Enter mantiene " + proyecto.Cliente.Id + "): ", proyecto.Cliente.Id);
            proyecto.Cliente = _servicioCliente.ObtenerClientePorId(nuevoClienteId);

            proyecto = LeerDatosProyecto(proyecto);
            proyecto.Estado = LeerEstadoConActual(proyecto.Estado);

            _servicioProyecto.ModificarProyecto(proyecto);
            Console.WriteLine("[OK] Proyecto modificado correctamente.");
        }

        private static Proyecto LeerDatosProyecto(Proyecto proyecto)
        {
            proyecto.Nombre = LeerTexto("Nombre del proyecto: ", obligatorio: true, actual: proyecto.Nombre);
            proyecto.Tipo = LeerTipoConActual(proyecto.Tipo);
            proyecto.FechaInicio = LeerFecha("Fecha de inicio (dd/mm/aaaa)", proyecto.FechaInicio);
            proyecto.PresupuestoInicial = LeerDecimal("Presupuesto inicial: ", proyecto.PresupuestoInicial);
            proyecto.FechaFinPlanificada = LeerFecha("Fecha de fin planificada (dd/mm/aaaa)", proyecto.FechaFinPlanificada);
            return proyecto;
        }

        private static void EliminarProyecto()
        {
            Console.WriteLine("\n--- Eliminar proyecto ---");
            int id = LeerEntero("Id del proyecto a eliminar: ");
            Proyecto proyecto = _servicioProyecto.ObtenerProyectoPorId(id);

            Console.WriteLine($"Se eliminará: {proyecto.Nombre}. ¿Confirma? (S/N): ");
            if (string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase))
            {
                _servicioProyecto.EliminarProyecto(id);
                Console.WriteLine("[OK] Proyecto eliminado.");
            }
            else
            {
                Console.WriteLine("Operación cancelada.");
            }
        }

        private static void ListarProyectos(List<Proyecto> proyectos, string titulo)
        {
            Console.WriteLine($"\n--- {titulo} ---");
            if (proyectos.Count == 0)
            {
                Console.WriteLine("No hay proyectos para mostrar.");
                return;
            }

            Console.WriteLine($"{"Id",-5} {"ClienteId",-10} {"Nombre",-35} {"Tipo",-14} {"Inicio",-12} {"Estado",-12} Presupuesto");
            Console.WriteLine(new string('-', 130));
            foreach (var p in proyectos)
            {
                Console.WriteLine($"{p.Id,-5} {p.Cliente.Id,-10} {p.Nombre,-35} {p.Tipo,-14} {p.FechaInicio:dd/MM/yyyy,-12} {p.Estado,-12} {p.PresupuestoInicial,12:N2}");
            }
        }

        private static void VerProyecto()
        {
            int id = LeerEntero("Id del proyecto: ");
            Proyecto p = _servicioProyecto.ObtenerProyectoPorId(id);
            Console.WriteLine($"\nId: {p.Id}");
            Console.WriteLine($"Nombre: {p.Nombre}");
            Console.WriteLine($"Cliente: {p.Cliente.Id}");
            Console.WriteLine($"Tipo: {p.Tipo}");
            Console.WriteLine($"Estado: {p.Estado}");
            Console.WriteLine($"Inicio: {p.FechaInicio:dd/MM/yyyy} | Fin planificado: {p.FechaFinPlanificada:dd/MM/yyyy} | Fin real: {(p.FechaFin.HasValue ? p.FechaFin.Value.ToString("dd/MM/yyyy") : "-")}");
            Console.WriteLine($"Presupuesto: {p.PresupuestoInicial:N2}");
        }

        // ==========================================
        // FACTURAS DE VENTA
        // ==========================================

        private static void MenuFacturasVenta()
        {
            bool volver = false;
            while (!volver)
            {
                Console.WriteLine();
                Console.WriteLine("----- FACTURAS DE VENTA -----");
                Console.WriteLine("1. Registrar factura de venta");
                Console.WriteLine("2. Listar todas las facturas");
                Console.WriteLine("3. Facturas por mes");
                Console.WriteLine("0. Volver");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": RegistrarFacturaVenta(); break;
                    case "2":
                        ListarFacturasVenta(_servicioFacturacion.ListarFacturasVentas(), "Todas las facturas de venta");
                        break;
                    case "3":
                        {
                            int mes = LeerEntero("Mes (1-12): ");
                            int anio = LeerEntero("Año: ");
                            ListarFacturasVenta(_servicioFacturacion.ListarVentasPorMes(mes, anio), $"Facturas de venta de {mes:D2}/{anio}");
                            break;
                        }
                    case "0": volver = true; break;
                    default: Console.WriteLine("Opción no válida."); break;
                }
            }
        }

        private static void RegistrarFacturaVenta()
        {
            Console.WriteLine("\n--- Registrar factura de venta ---");

            FacturaVenta factura = new FacturaVenta
            {
                NroFactura = LeerTexto("Número de factura: ", obligatorio: true),
                Fecha = LeerFechaOpcional("Fecha (dd/mm/aaaa, Enter = hoy)", DateTime.Now),
                Cliente = _servicioCliente.ObtenerClientePorId(LeerEntero("Id del cliente: "))
            };

            int proyectoId = LeerEnteroOpcional("Id del proyecto (opcional, Enter para omitir): ");
            if (proyectoId > 0)
            {
                factura.Proyecto = _servicioProyecto.ObtenerProyectoPorId(proyectoId);
            }

            bool llevaIva = LeerSiNo("¿La factura lleva IVA? (S/N): ");

            Console.WriteLine("\nCargue los renglones de detalle. Escriba 'FIN' en la descripción para terminar.");
            while (true)
            {
                string descripcion = LeerTexto("Descripción del renglón: ", obligatorio: false);
                if (string.IsNullOrWhiteSpace(descripcion) ||
                    string.Equals(descripcion.Trim(), "FIN", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                decimal monto = LeerDecimal("Monto: ", 0);
                factura.Detalles.Add(new DetalleFacturaVenta { Descripcion = descripcion.Trim(), Monto = monto });
            }

            _servicioFacturacion.RegistrarFacturaVenta(factura, llevaIva);
            Console.WriteLine($"[OK] Factura {factura.NroFactura} registrada. SubTotal: {factura.SubTotal:N2} | IVA: {factura.IVA:N2} | Total: {factura.MontoTotal:N2}");
        }

        private static void ListarFacturasVenta(List<FacturaVenta> facturas, string titulo)
        {
            Console.WriteLine($"\n--- {titulo} ---");
            if (facturas.Count == 0)
            {
                Console.WriteLine("No hay facturas para mostrar.");
                return;
            }

            Console.WriteLine($"{"NroFactura",-12} {"Fecha",-12} {"ClienteId",-10} {"ProyectoId",-11} {"SubTotal",-12} {"IVA",-10} {"Total",-12} Detalles");
            Console.WriteLine(new string('-', 120));
            foreach (var f in facturas)
            {
                Console.WriteLine($"{f.NroFactura,-12} {f.Fecha:dd/MM/yyyy,-12} {f.Cliente.Id,-10} {(f.Proyecto?.Id.ToString() ?? "-"),-11} {f.SubTotal,12:N2} {f.IVA,10:N2} {f.MontoTotal,12:N2} {f.Detalles.Count}");
            }
        }

        // ==========================================
        // FACTURAS DE COMPRA
        // ==========================================

        private static void MenuFacturasCompra()
        {
            bool volver = false;
            while (!volver)
            {
                Console.WriteLine();
                Console.WriteLine("----- FACTURAS DE COMPRA -----");
                Console.WriteLine("1. Registrar factura de compra");
                Console.WriteLine("2. Listar todas las facturas");
                Console.WriteLine("3. Facturas por mes");
                Console.WriteLine("0. Volver");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": RegistrarFacturaCompra(); break;
                    case "2":
                        ListarFacturasCompra(_servicioFacturacion.ListarFacturasCompras(), "Todas las facturas de compra");
                        break;
                    case "3":
                        {
                            int mes = LeerEntero("Mes (1-12): ");
                            int anio = LeerEntero("Año: ");
                            ListarFacturasCompra(_servicioFacturacion.ListarComprasPorMes(mes, anio), $"Facturas de compra de {mes:D2}/{anio}");
                            break;
                        }
                    case "0": volver = true; break;
                    default: Console.WriteLine("Opción no válida."); break;
                }
            }
        }

        private static void RegistrarFacturaCompra()
        {
            Console.WriteLine("\n--- Registrar factura de compra ---");

            FacturaCompra factura = new FacturaCompra
            {
                NroFactura = LeerTexto("Número de factura: ", obligatorio: true),
                Fecha = LeerFechaOpcional("Fecha (dd/mm/aaaa, Enter = hoy)", DateTime.Now),
                ProveedorRazonSocial = LeerTexto("Razón Social del proveedor: ", obligatorio: true),
                ProveedorRUT = LeerTexto("RUT del proveedor (12 dígitos): ", obligatorio: true),
                ProveedorDireccion = LeerTexto("Dirección del proveedor: ", obligatorio: true),
                SubTotal = LeerDecimal("Subtotal: ", 0),
                IVA = LeerDecimal("IVA: ", 0)
            };

            _servicioFacturacion.RegistrarFacturaCompra(factura);
            Console.WriteLine($"[OK] Factura de compra {factura.NroFactura} registrada. Total: {factura.MontoTotal:N2}");
        }

        private static void ListarFacturasCompra(List<FacturaCompra> facturas, string titulo)
        {
            Console.WriteLine($"\n--- {titulo} ---");
            if (facturas.Count == 0)
            {
                Console.WriteLine("No hay facturas para mostrar.");
                return;
            }

            Console.WriteLine($"{"NroFactura",-12} {"Fecha",-12} {"Proveedor",-35} {"SubTotal",-12} {"IVA",-10} {"Total",-12}");
            Console.WriteLine(new string('-', 110));
            foreach (var f in facturas)
            {
                Console.WriteLine($"{f.NroFactura,-12} {f.Fecha:dd/MM/yyyy,-12} {f.ProveedorRazonSocial,-35} {f.SubTotal,12:N2} {f.IVA,10:N2} {f.MontoTotal,12:N2}");
            }
        }

        // ==========================================
        // IMPUESTOS
        // ==========================================

        private static void MenuImpuestos()
        {
            bool volver = false;
            while (!volver)
            {
                Console.WriteLine();
                Console.WriteLine("----- IMPUESTOS -----");
                Console.WriteLine("1. Calcular y guardar liquidación mensual");
                Console.WriteLine("2. Consultar liquidación por mes");
                Console.WriteLine("0. Volver");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        {
                            int mes = LeerEntero("Mes (1-12): ");
                            int anio = LeerEntero("Año: ");
                            LiquidacionImpuestos liq = _servicioImpuestos.CalcularYGuardarLiquidacionMensual(mes, anio);
                            MostrarLiquidacion(liq);
                            break;
                        }
                    case "2":
                        {
                            int mes = LeerEntero("Mes (1-12): ");
                            int anio = LeerEntero("Año: ");
                            LiquidacionImpuestos liq = _servicioImpuestos.ConsultarLiquidacionPorMes(mes, anio);
                            MostrarLiquidacion(liq);
                            break;
                        }
                    case "0": volver = true; break;
                    default: Console.WriteLine("Opción no válida."); break;
                }
            }
        }

        private static void MostrarLiquidacion(LiquidacionImpuestos liq)
        {
            Console.WriteLine($"\n--- Liquidación {liq.Mes:D2}/{liq.Anio} ---");
            Console.WriteLine($"Total facturado:      {liq.TotalFacturado,15:N2}");
            Console.WriteLine($"IVA ventas:           {liq.TotalIvaVentas,15:N2}");
            Console.WriteLine($"IVA compras:          {liq.TotalIvaCompras,15:N2}");
            Console.WriteLine($"Monto IVA (saldo):    {liq.MontoIva,15:N2}");
            Console.WriteLine($"IRAE (12% factur.):   {liq.MontoIrae,15:N2}");
        }

        // ==========================================
        // HELPERS DE ENTRADA
        // ==========================================

        private static string LeerTexto(string mensaje, bool obligatorio, string? actual = null)
        {
            while (true)
            {
                Console.Write(mensaje);
                string valor = Console.ReadLine()?.Trim() ?? string.Empty;

                if (!string.IsNullOrEmpty(valor))
                {
                    return valor;
                }

                if (!obligatorio && actual != null)
                {
                    return actual;
                }

                if (!obligatorio && actual == null)
                {
                    return string.Empty;
                }

                Console.WriteLine("Este dato es obligatorio.");
            }
        }

        private static int LeerEntero(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                if (int.TryParse(Console.ReadLine()?.Trim(), out int valor))
                {
                    return valor;
                }
                Console.WriteLine("Ingrese un número entero válido.");
            }
        }

        private static int LeerEnteroOpcional(string mensaje, int minimo = 0)
        {
            Console.Write(mensaje);
            string texto = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(texto))
            {
                return 0;
            }
            return int.TryParse(texto, out int valor) ? valor : minimo;
        }

        private static int LeerEnteroConActual(string mensaje, int actual)
        {
            Console.Write(mensaje);
            string texto = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(texto))
            {
                return actual;
            }
            while (!int.TryParse(texto, out int valor))
            {
                Console.WriteLine("Ingrese un número entero válido.");
                Console.Write(mensaje);
                texto = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(texto))
                {
                    return actual;
                }
            }
            return int.Parse(texto);
        }

        private static decimal LeerDecimal(string mensaje, decimal actual = 0)
        {
            while (true)
            {
                Console.Write(mensaje);
                string texto = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(texto))
                {
                    return actual;
                }
                // Acepta coma o punto como separador decimal
                if (decimal.TryParse(texto.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
                {
                    return valor;
                }
                Console.WriteLine("Ingrese un número válido (ej: 1234.56).");
            }
        }

        private static DateTime LeerFecha(string mensaje, DateTime actual)
        {
            while (true)
            {
                Console.Write(mensaje + $" [{actual:dd/MM/yyyy}]: ");
                string texto = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(texto))
                {
                    return actual;
                }
                if (DateTime.TryParse(texto, out DateTime valor))
                {
                    return valor;
                }
                Console.WriteLine("Ingrese una fecha válida (ej: 15/03/2025).");
            }
        }

        private static DateTime LeerFechaOpcional(string mensaje, DateTime porDefecto)
        {
            Console.Write(mensaje + ": ");
            string texto = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(texto))
            {
                return porDefecto;
            }
            while (!DateTime.TryParse(texto, out DateTime valor))
            {
                Console.WriteLine("Ingrese una fecha válida (ej: 15/03/2025) o Enter para usar la fecha actual.");
                Console.Write(mensaje + ": ");
                texto = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(texto))
                {
                    return porDefecto;
                }
            }
            return DateTime.Parse(texto);
        }

        private static TipoProyecto LeerTipoConActual(TipoProyecto actual)
        {
            Console.WriteLine("Tipos de proyecto: 0=PorHora | 1=MontoMensual | 2=PrecioFijo");
            Console.Write($"Tipo [{(int)actual}] (Enter mantiene): ");
            string texto = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(texto))
            {
                return actual;
            }
            while (!Enum.TryParse<TipoProyecto>(texto, out TipoProyecto tipo) || !Enum.IsDefined(typeof(TipoProyecto), tipo))
            {
                Console.WriteLine("Ingrese 0, 1 o 2.");
                Console.Write("Tipo: ");
                texto = Console.ReadLine()?.Trim() ?? string.Empty;
            }
            return Enum.Parse<TipoProyecto>(texto);
        }

        private static EstadoProyecto LeerEstadoConActual(EstadoProyecto actual)
        {
            Console.WriteLine("Estados: 0=Planificado | 1=EnCurso | 2=Finalizado | 3=Cancelado");
            Console.Write($"Estado [{(int)actual}] (Enter mantiene): ");
            string texto = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(texto))
            {
                return actual;
            }
            while (!Enum.TryParse<EstadoProyecto>(texto, out EstadoProyecto estado) || !Enum.IsDefined(typeof(EstadoProyecto), estado))
            {
                Console.WriteLine("Ingrese un valor entre 0 y 3.");
                Console.Write("Estado: ");
                texto = Console.ReadLine()?.Trim() ?? string.Empty;
            }
            return Enum.Parse<EstadoProyecto>(texto);
        }

        private static bool LeerSiNo(string mensaje)
        {
            Console.Write(mensaje);
            string texto = Console.ReadLine()?.Trim() ?? string.Empty;
            return string.Equals(texto, "S", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(texto, "SI", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(texto, "SÍ", StringComparison.OrdinalIgnoreCase);
        }

        private static void Pausa()
        {
            Console.WriteLine("\nPresione Enter para continuar...");
            Console.ReadLine();
        }
    }
}
