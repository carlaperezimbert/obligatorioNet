using CapaDominio;

namespace CapaLogica
{
    public interface IServicioCliente
    {
        void AltaCliente(Cliente cliente);
        void EliminarCliente(int idCliente);
        List<Cliente> ListarClientes();
        void ModificarCliente(Cliente cliente);
        Cliente ObtenerClientePorId(int idCliente);
    }
}