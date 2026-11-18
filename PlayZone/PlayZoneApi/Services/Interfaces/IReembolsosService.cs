using PlayZoneApi.DTOs.ReembolsoDTOs;
namespace PlayZoneApi.Services.Interfaces
{
    public interface IReembolsosService
    {
        Task<ReembolsoDTO> CrearAsync(ReembolsoCrearDTO dto);

        Task<List<ReembolsoDTO>> ObtenerTodosAsync();

        Task<ReembolsoDTO> ObtenerPorIdAsync(int id);

        Task<ReembolsoDTO> ActualizarAsync(int id, ReembolsoActualizarDTO dto);


    }
}
