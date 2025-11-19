using GymTrainerGuide.Api.Data;
using GymTrainerGuide.Api.Helpers;
using GymTrainerGuide.Shared.DTOs;
using GymTrainerGuide.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymTrainerGuide.Api.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<string>> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UserHelper(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<string>> roleManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                var newRole = new IdentityRole<string>
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };

                await _roleManager.CreateAsync(newRole);
            }
        }


        public async Task<User> GetUserAsync(string email)
        {

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email! == email);

            return user!;

        }


        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginDTO model)
        {
            return await _signInManager.PasswordSignInAsync(model.Correo, model.Password, false, false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

    }
}
