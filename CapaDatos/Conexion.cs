using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public static class Conexion
    {
        private static readonly string _connectionString;
        static Conexion()
        {
            // Obtiene la cadena de conexión desde App.config o appsettings.json
            _connectionString =

            ConfigurationManager.ConnectionStrings["ObligatorioConnection"]?.ConnectionString

            ??

            "Server=(localdb)\\mssqllocaldb;Database=ObligatorioDB;Trusted_Connection=True;TrustServerCertificate=True;";
        }
        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
