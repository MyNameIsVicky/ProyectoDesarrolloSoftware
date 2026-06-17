using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoDesarrolloSoftware.Data;

namespace ProyectoDesarrolloSoftware.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicoApiController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public MedicoApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var medico = await _context.Medicos.FindAsync(id);
            if (medico == null) return NotFound();

            _context.Medicos.Remove(medico);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
