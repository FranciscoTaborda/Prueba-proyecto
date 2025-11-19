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
    public class ObjetivoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ObjetivoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Objetivo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Objetivo>>> GetObjetivos()
        {
            return await _context.Objetivos.ToListAsync();
        }

        // GET: api/Objetivo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Objetivo>> GetObjetivo(int id)
        {
            var objetivo = await _context.Objetivos.FindAsync(id);

            if (objetivo == null)
            {
                return NotFound();
            }

            return objetivo;
        }

        // PUT: api/Objetivo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutObjetivo(int id, Objetivo objetivo)
        {
            if (id != objetivo.Id)
            {
                return BadRequest();
            }

            _context.Entry(objetivo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ObjetivoExists(id))
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

        // POST: api/Objetivo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Objetivo>> PostObjetivo(Objetivo objetivo)
        {
            _context.Objetivos.Add(objetivo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetObjetivo", new { id = objetivo.Id }, objetivo);
        }

        // DELETE: api/Objetivo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteObjetivo(int id)
        {
            var objetivo = await _context.Objetivos.FindAsync(id);
            if (objetivo == null)
            {
                return NotFound();
            }

            _context.Objetivos.Remove(objetivo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ObjetivoExists(int id)
        {
            return _context.Objetivos.Any(e => e.Id == id);
        }
    }
}
