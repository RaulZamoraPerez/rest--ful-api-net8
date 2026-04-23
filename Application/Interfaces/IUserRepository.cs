using ApiEcommerce.Domain.Entities;
using ApiEcommerce.Application.DTOs;

namespace ApiEcommerce.Application.Interfaces
{
    /// <summary>
    /// Interfaz para repositorio de usuarios en Clean Architecture
    /// </summary>
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(CreateUserDto createUserDto);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<List<User>> GetAllAsync();
    }
}