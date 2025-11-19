using GymTrainerGuide.Api.Helpers;
using GymTrainerGuide.Shared.DTOs;
using GymTrainerGuide.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTrainerGuide.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class AdminController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly UserManager<User> _userManager;

        public AdminController(IUserHelper userHelper, UserManager<User> userManager)
        {
            _userHelper = userHelper;
            _userManager = userManager;
        }

        // GET: api/Admin/Users
        [HttpGet("Users")]
        public async Task<ActionResult<IEnumerable<UserEditDTO>>> GetUsers()
        {
            // Obtener todos los usuarios y mapearlos a UserEditDTO
            var users = await _userManager.Users
                .Select(u => new UserEditDTO
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Edad = u.Edad,
                    Genero = u.Genero,
                    Correo = u.Correo,
                    TipoUsuario = u.TipoUsuario,
                    NivelExperienciaId = u.NivelExperienciaId,
                    ObjetivoId = u.ObjetivoId
                })
                .ToListAsync();

            return Ok(users);
        }

        // PUT: api/Admin/User
        [HttpPut("User")]
        public async Task<ActionResult> PutUser(UserEditDTO model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // 1. Actualizar las propiedades de la entidad User
            user.Nombre = model.Nombre;
            user.Edad = model.Edad;
            user.Genero = model.Genero;
            user.NivelExperienciaId = model.NivelExperienciaId;
            user.ObjetivoId = model.ObjetivoId;
            user.TipoUsuario = model.TipoUsuario;

            // 2. Actualizar el Correo/UserName (si ha cambiado)
            if (user.Correo != model.Correo)
            {
                user.Correo = model.Correo;
                user.Email = model.Correo;
                user.UserName = model.Correo;

                // Si el correo cambia, debe actualizarse en Identity
                var emailResult = await _userManager.UpdateAsync(user);
                if (!emailResult.Succeeded)
                {
                    return BadRequest(emailResult.Errors.FirstOrDefault()?.Description);
                }
            }

            // 3. Actualizar el resto de las propiedades
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }

            // 4. Actualizar el rol (si ha cambiado)
            var currentRoles = await _userManager.GetRolesAsync(user);
            var newRole = model.TipoUsuario.ToString();

            if (!currentRoles.Contains(newRole))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, newRole);
            }

            return NoContent();
        }

        // DELETE: api/Admin/User/id
        [HttpDelete("User/{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }

            return NoContent();
        }
    }
}
