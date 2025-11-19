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
    public class RutinaEjerciciosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RutinaEjerciciosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RutinaEjercicios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RutinaEjercicio>>> GetRutinaEjercicios()
        {
            return await _context.RutinaEjercicios.ToListAsync();
        }

        // GET: api/RutinaEjercicios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RutinaEjercicio>> GetRutinaEjercicio(int id)
        {
            var rutinaEjercicio = await _context.RutinaEjercicios.FindAsync(id);

            if (rutinaEjercicio == null)
            {
                return NotFound();
            }

            return rutinaEjercicio;
        }

        // PUT: api/RutinaEjercicios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRutinaEjercicio(int id, RutinaEjercicio rutinaEjercicio)
        {
            if (id != rutinaEjercicio.Id)
            {
                return BadRequest();
            }

            _context.Entry(rutinaEjercicio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RutinaEjercicioExists(id))
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

        // POST: api/RutinaEjercicios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RutinaEjercicio>> PostRutinaEjercicio(RutinaEjercicio rutinaEjercicio)
        {
            _context.RutinaEjercicios.Add(rutinaEjercicio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRutinaEjercicio", new { id = rutinaEjercicio.Id }, rutinaEjercicio);
        }

        // DELETE: api/RutinaEjercicios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRutinaEjercicio(int id)
        {
            var rutinaEjercicio = await _context.RutinaEjercicios.FindAsync(id);
            if (rutinaEjercicio == null)
            {
                return NotFound();
            }

            _context.RutinaEjercicios.Remove(rutinaEjercicio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RutinaEjercicioExists(int id)
        {
            return _context.RutinaEjercicios.Any(e => e.Id == id);
        }
    }
}
