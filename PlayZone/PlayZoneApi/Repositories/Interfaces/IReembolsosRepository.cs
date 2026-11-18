
using PlayZoneApi.Models;

namespace PlayZoneApi.Repositories.Interfaces
{
    public interface IReembolsosRepository
    {
        // Obtener detalle de orden con su orden asociada.
        Task<Detalle_Orden?> ObtenerDetalleOrdenAsync(int idDetalleOrden);

        // Verificar si ya existe un reembolso para ese detalle.
        Task<bool> ExisteReembolsoAsync(int idDetalleOrden);

        // Obtener estado de reembolso por nombre.
        Task<Estados_Reembolso?> ObtenerEstadoReembolsoAsync(string nombre);

        // Crear un reembolso.
        Task<Reembolso> CrearAsync(Reembolso reembolso);
        // obtener reembolsos
        Task<List<Reembolso>> ObtenerTodosAsync();

        Task<Reembolso?> ObtenerPorIdAsync(int id);

        Task<Reembolso?> ObtenerPorIdParaActualizarAsync(int id);

        Task<Reembolso> ActualizarAsync(Reembolso reembolso);

        Task<List<Detalle_Orden>> ObtenerDetallesPorOrdenAsync(int idOrden);

        Task ActualizarReembolsoYOrdenAsync(Reembolso reembolso, Ordenes_Compra? orden);
    }
}
