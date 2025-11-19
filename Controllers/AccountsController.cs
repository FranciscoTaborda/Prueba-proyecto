using GymTrainerGuide.Api.Helpers;
using GymTrainerGuide.Shared.DTOs;
using GymTrainerGuide.Shared.DTOs;
using GymTrainerGuide.Shared.Entities;
using GymTrainerGuide.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace GymTrainerGuide.Api.Controllers
{
    [ApiController]
    [Route("/api/accounts")]
    public class AccountsController : ControllerBase
    {
    
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;

        public AccountsController(IUserHelper userHelper, IConfiguration configuration)
        {
            _userHelper = userHelper;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] UserDTO model)
        {
            User user = model;
            user.Email = user.Correo;
            user.UserName = user.Correo;

            var result = await _userHelper.AddUserAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userHelper.AddUserToRoleAsync(user, user.TipoUsuario.ToString());
                return Ok(BuildToken(user));
            }

            // Obtener el primer error
            var error = result.Errors.FirstOrDefault();

            if (error != null)
            {
                // Personalizamos según el código
                switch (error.Code)
                {
                    case "DuplicateUserName":
                    case "DuplicateEmail":
                        error.Description = "Este correo ya está registrado, por favor usa uno diferente.";
                        break;

                    case "PasswordTooShort":
                        error.Description = "La contraseña es demasiado corta. Debe cumplir los requisitos mínimo (Digitos).";
                        break;

                    /*case "PasswordRequiresDigit":
                        error.Description = "La contraseña debe incluir al menos un número.";
                        break;

                    case "PasswordRequiresUpper":
                        error.Description = "La contraseña debe incluir al menos una letra mayúscula.";
                        break;

                        // Puedes agregar más según lo que necesites*/  
                }
            }

            return BadRequest(error.Description);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO model)
        {
            var result = await _userHelper.LoginAsync(model);
            if (result.Succeeded)
            {
                var user = await _userHelper.GetUserAsync(model.Correo);
                return Ok(BuildToken(user));
            }

            return BadRequest("Email o contraseña incorrectos.");
        }

        private TokenDTO BuildToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nombre), 
                new Claim(ClaimTypes.Role, user.TipoUsuario.ToString()),
                new Claim(ClaimTypes.Email, user.Correo),
                new Claim("Edad", user.Edad.ToString()),
                new Claim("Genero", user.Genero.ToString()),
                new Claim("NivelExperienciaId", user.NivelExperienciaId.ToString()),
                new Claim("ObjetivoId", user.ObjetivoId.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(30);
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
