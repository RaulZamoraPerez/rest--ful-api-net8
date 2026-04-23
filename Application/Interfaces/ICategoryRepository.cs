using DomainCategory = ApiEcommerce.Domain.Entities.Category;

namespace ApiEcommerce.Application.Interfaces
{
    /// <summary>
    /// Contrato del repositorio de categorías
    /// ✅ Solo trabaja con entidades de dominio
    /// ✅ Define las operaciones necesarias para el negocio
    /// </summary>
    public interface ICategoryRepository
    {
        // Consultas básicas
        Task<IEnumerable<DomainCategory>> GetAllAsync();
        Task<DomainCategory?> GetByIdAsync(int id);
        Task<DomainCategory?> GetByNameAsync(string name);
        
        // Operaciones CRUD
        Task<DomainCategory> CreateAsync(DomainCategory category);
        Task<DomainCategory> UpdateAsync(DomainCategory category);
        Task<bool> DeleteAsync(int id);
        
        // Consultas de negocio
        Task<bool> ExistsAsync(int id);
        Task<bool> NameExistsAsync(string name);
        Task<bool> NameExistsAsync(string name, int excludeCategoryId);
        Task<bool> HasProductsAsync(int categoryId);
        
        // Estadísticas
        Task<int> GetProductCountByCategoryAsync(int categoryId);
        Task<IEnumerable<(DomainCategory Category, int ProductCount)>> GetCategoriesWithProductCountAsync();
    }
}