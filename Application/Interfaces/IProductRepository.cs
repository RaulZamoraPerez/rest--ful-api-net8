using DomainProduct = ApiEcommerce.Domain.Entities.Product;

namespace ApiEcommerce.Application.Interfaces
{
    /// <summary>
    /// Contrato del repositorio de productos
    /// ✅ Está en Application (no en Domain) porque define CÓMO acceder a datos
    /// ✅ Solo trabaja con entidades de dominio
    /// ✅ No tiene implementación - solo el contrato
    /// </summary>
    public interface IProductRepository
    {
        // Consultas básicas
        Task<IEnumerable<DomainProduct>> GetAllAsync();
        Task<DomainProduct?> GetByIdAsync(int id);
        Task<DomainProduct?> GetBySkuAsync(string sku);
        
        // Consultas con filtros
        Task<IEnumerable<DomainProduct>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<DomainProduct>> SearchAsync(string searchTerm);
        Task<IEnumerable<DomainProduct>> GetProductsAsync(string? search, int? categoryId);
        
        // Operaciones CRUD
        Task<DomainProduct> CreateAsync(DomainProduct product);
        Task<DomainProduct> UpdateAsync(DomainProduct product);
        Task<bool> DeleteAsync(int id);
        
        // Consultas de negocio
        Task<bool> ExistsAsync(int id);
        Task<bool> SkuExistsAsync(string sku);
        Task<bool> SkuExistsAsync(string sku, int excludeProductId);
        Task<IEnumerable<DomainProduct>> GetLowStockProductsAsync(int threshold = 5);
        Task<IEnumerable<DomainProduct>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        
        // Paginación
        Task<(IEnumerable<DomainProduct> Products, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string? search = null, int? categoryId = null);
    }
}