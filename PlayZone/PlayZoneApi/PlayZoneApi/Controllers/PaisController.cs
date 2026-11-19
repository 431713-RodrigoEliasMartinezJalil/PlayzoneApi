using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayZoneApi.DTOs;
using PlayZoneApi.Models;

namespace PlayZoneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaisController : ControllerBase
    {
        private readonly PlayZoneContext _context;

        public PaisController(PlayZoneContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaisDTO>>> ObtenerTodos()
        {
            var paises = await _context.Pais
                .AsNoTracking()
                .OrderBy(p => p.nombre)
                .Select(p => new PaisDTO
                {
                    IdPais = p.id_pais,
                    Nombre = p.nombre
                })
                .ToListAsync();

            return Ok(paises);
        }
    }
}
