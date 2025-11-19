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
    public class RutinasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RutinasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Rutinas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rutina>>> GetRutinas()
        {
            return await _context.Rutinas.ToListAsync();
        }

        // GET: api/Rutinas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rutina>> GetRutina(int id)
        {
            var rutina = await _context.Rutinas.FindAsync(id);

            if (rutina == null)
            {
                return NotFound();
            }

            return rutina;
        }

        // PUT: api/Rutinas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRutina(int id, Rutina rutina)
        {
            if (id != rutina.Id)
            {
                return BadRequest();
            }

            _context.Entry(rutina).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RutinaExists(id))
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

        // POST: api/Rutinas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Rutina>> PostRutina(Rutina rutina)
        {
            _context.Rutinas.Add(rutina);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRutina", new { id = rutina.Id }, rutina);
        }

        // DELETE: api/Rutinas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRutina(int id)
        {
            var rutina = await _context.Rutinas.FindAsync(id);
            if (rutina == null)
            {
                return NotFound();
            }

            _context.Rutinas.Remove(rutina);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RutinaExists(int id)
        {
            return _context.Rutinas.Any(e => e.Id == id);
        }
    }
}
