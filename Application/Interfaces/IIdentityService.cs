using ApiEcommerce.Models;

namespace ApiEcommerce.Application.Interfaces
{
    /// <summary>
    /// Interfaz para servicios de Identity - DEBE estar en Application
    /// ✅ Define contratos sin implementación
    /// ✅ No depende de tecnologías específicas
    /// </summary>
    public interface IIdentityService
    {
        Task<(bool Success, string Message, string? Token)> LoginAsync(string email, string password);
        Task<(bool Success, string Message)> RegisterAsync(string email, string password, string name, string role = "User");
        Task<(bool Success, string Message)> CreateRoleAsync(string roleName);
        Task<(bool Success, string Message)> AssignRoleAsync(string email, string role);
        Task<AplicationUser?> GetUserByEmailAsync(string email);
        Task<IEnumerable<string>> GetUserRolesAsync(string email);
    }
}