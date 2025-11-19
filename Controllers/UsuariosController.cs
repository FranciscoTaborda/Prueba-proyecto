using GymTrainerGuide.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymTrainerGuide.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuariosController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsuariosController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // ✅ GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        // ✅ GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // ✅ POST: api/Usuarios
        // Crear un usuario (sin roles ni contraseña de momento)
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // si quieres, puedes validar que no haya otro correo igual
            if (await _userManager.FindByEmailAsync(user.Correo) != null)
            {
                return BadRequest("Ya existe un usuario con este correo.");
            }

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // ✅ PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.Nombre = user.Nombre;
            existingUser.Edad = user.Edad;
            existingUser.Genero = user.Genero;
            existingUser.Correo = user.Correo;
            existingUser.TipoUsuario = user.TipoUsuario;
            existingUser.NivelExperienciaId = user.NivelExperienciaId;
            existingUser.ObjetivoId = user.ObjetivoId;

            var result = await _userManager.UpdateAsync(existingUser);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        // ✅ DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(string id)
        {
            var usuario = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(usuario);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}
