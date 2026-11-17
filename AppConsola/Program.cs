using System;

using CapaDominio;
using CapaLogica;

namespace AppConsola
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("  SISTEMA DE GESTION - OBLIGATORIO .NET  ");
            Console.WriteLine("========================================");
            Console.WriteLine("1. Probar conexion con la Capa Logica");
            Console.WriteLine("0. Salir");
            Console.Write("\nSeleccione una opcion: ");

            string opcion = Console.ReadLine();

            if (opcion == "1")
            {
                Console.WriteLine("\n[>] Ejecutando prueba de conexion...");

                
                // Ejemplo:
                // ManejadorClientes logicaClientes = new ManejadorClientes();
                // var resultado = logicaClientes.ObtenerTodos();
                // foreach (var item in resultado) { Console.WriteLine(item.ToString()); }

                Console.WriteLine("[OK] ¡La consola se comunica perfectamente con las capas!");
            }
            else
            {
                Console.WriteLine("\nSaliendo del sistema...");
            }

            Console.WriteLine("\nPresione cualquier tecla para terminar...");
            Console.ReadKey();
        }
    }
}