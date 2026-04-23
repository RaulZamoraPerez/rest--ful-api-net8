using ApiEcommerce.Application.Interfaces;
using ApiEcommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiEcommerce.Infrastructure.Services
{
    /// <summary>
    /// Servicio de Identity en Infrastructure
    /// ✅ Implementa IIdentityService de Application
    /// ✅ Maneja autenticación y autorización
    /// ✅ Genera JWT tokens
    /// ✅ Interactúa con Identity Framework
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<AplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public IdentityService(
            UserManager<AplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message, string? Token)> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "Usuario no encontrado", null);

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
                return (false, "Credenciales incorrectas", null);

            var token = await GenerateJwtTokenAsync(user);
            return (true, "Login exitoso", token);
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string email, string password, string name, string role = "User")
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return (false, "El usuario ya existe");

            var user = new AplicationUser
            {
                UserName = email,
                Email = email,
                Name = name
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Error al crear usuario: {errors}");
            }

            // Asignar rol
            if (await _roleManager.RoleExistsAsync(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            return (true, "Usuario creado exitosamente");
        }

        public async Task<(bool Success, string Message)> CreateRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                return (false, "El rol ya existe");

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Error al crear rol: {errors}");
            }

            return (true, "Rol creado exitosamente");
        }

        public async Task<(bool Success, string Message)> AssignRoleAsync(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "Usuario no encontrado");

            if (!await _roleManager.RoleExistsAsync(role))
                return (false, "El rol no existe");

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Error al asignar rol: {errors}");
            }

            return (true, "Rol asignado exitosamente");
        }

        public async Task<AplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new List<string>();

            return await _userManager.GetRolesAsync(user);
        }

        private async Task<string> GenerateJwtTokenAsync(AplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? ""),
                new(ClaimTypes.Email, user.Email ?? ""),
                new("Name", user.Name ?? "")
            };

            // Agregar roles como claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JWT:Key"] ?? throw new InvalidOperationException("JWT Key no configurada")));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7), // Token válido por 7 días
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}