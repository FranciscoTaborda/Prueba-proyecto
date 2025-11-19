using GymTrainerGuide.Api.Data;
using GymTrainerGuide.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymTrainerGuide.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NivelExperienciasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NivelExperienciasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/NivelExperiencias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NivelExperiencia>>> GetNivelesExperiencia()
        {
            return await _context.NivelExperiencia.ToListAsync();
        }

        // GET: api/NivelExperiencias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NivelExperiencia>> GetNivelExperiencia(int id)
        {
            var nivelExperiencia = await _context.NivelExperiencia.FindAsync(id);

            if (nivelExperiencia == null)
            {
                return NotFound();
            }

            return nivelExperiencia;
        }

        // PUT: api/NivelExperiencias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNivelExperiencia(int id, NivelExperiencia nivelExperiencia)
        {
            if (id != nivelExperiencia.Id)
            {
                return BadRequest();
            }

            _context.Entry(nivelExperiencia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NivelExperienciaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/NivelExperiencias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NivelExperiencia>> PostNivelExperiencia(NivelExperiencia nivelExperiencia)
        {
            _context.NivelExperiencia.Add(nivelExperiencia);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNivelExperiencia", new { id = nivelExperiencia.Id }, nivelExperiencia);
        }

        // DELETE: api/NivelExperiencias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNivelExperiencia(int id)
        {
            var nivelExperiencia = await _context.NivelExperiencia.FindAsync(id);
            if (nivelExperiencia == null)
            {
                return NotFound();
            }

            _context.NivelExperiencia.Remove(nivelExperiencia);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NivelExperienciaExists(int id)
        {
            return _context.NivelExperiencia.Any(e => e.Id == id);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<NivelExperiencia>>> GetAllNiveles()
        {
            return Ok(await _context.NivelExperiencia.ToListAsync());
        }
    }
}
