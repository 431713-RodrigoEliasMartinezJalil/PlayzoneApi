namespace PlayZoneApi.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        // RESUMEN DE ORDENES
        Task<int> ObtenerTotalOrdenesAsync();

        Task<int> ObtenerOrdenesPorEstadoAsync(string estado);

        // INFORMACION GENERAL
        Task<int> ObtenerTotalProductosAsync();

        Task<int> ObtenerTotalUsuariosAsync();

        // REEMBOLSOS
        Task<int> ObtenerTotalReembolsosAsync();
    }
}
