using ApiEcommerce.Application.Interfaces;
using ApiEcommerce.Application.DTOs;
using DomainProduct = ApiEcommerce.Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Infrastructure.Persistence
{
    /// <summary>
    /// Implementación del repositorio de productos en Infrastructure
    /// ✅ Implementa la interfaz definida en Application
    /// ✅ Usa Entity Framework para persistencia
    /// ✅ Trabaja directamente con entidades Domain
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Consultas básicas

        public async Task<IEnumerable<DomainProduct>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<DomainProduct?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<DomainProduct?> GetBySkuAsync(string sku)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.SKU == sku);
        }

        #endregion

        #region Consultas con filtros

        public async Task<IEnumerable<DomainProduct>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DomainProduct>> SearchAsync(string searchTerm)
        {
            return await _context.Products
                .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<DomainProduct>> GetProductsAsync(string? search, int? categoryId)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return await query.ToListAsync();
        }

        #endregion

        #region Operaciones de escritura

        public async Task<DomainProduct> CreateAsync(CreateProductDto createProductDto)
        {
            // 🎯 Usar factory method del dominio
            var product = DomainProduct.Create(
                createProductDto.Name,
                createProductDto.Description,
                createProductDto.Price,
                createProductDto.SKU,
                createProductDto.Stock,
                createProductDto.CategoryId,
                createProductDto.ImgUrl,
                createProductDto.ImgUrlLocal
            );

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<DomainProduct> CreateAsync(DomainProduct product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<DomainProduct> UpdateAsync(DomainProduct product)
        {
            var existingProduct = await _context.Products.FindAsync(product.ProductId);
            if (existingProduct == null)
                throw new ArgumentException($"Producto con ID {product.ProductId} no encontrado");

            // Actualizar usando EF Core change tracking
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.SKU = product.SKU;
            existingProduct.Stock = product.Stock;
            existingProduct.ImgUrl = product.ImgUrl;
            existingProduct.ImgUrlLocal = product.ImgUrlLocal;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.UpdateDate = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Consultas de negocio

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(p => p.ProductId == id);
        }

        public async Task<bool> SkuExistsAsync(string sku)
        {
            return await _context.Products.AnyAsync(p => p.SKU == sku);
        }

        public async Task<bool> SkuExistsAsync(string sku, int excludeProductId)
        {
            return await _context.Products.AnyAsync(p => p.SKU == sku && p.ProductId != excludeProductId);
        }

        public async Task<IEnumerable<DomainProduct>> GetLowStockProductsAsync(int threshold = 5)
        {
            return await _context.Products
                .Where(p => p.Stock <= threshold)
                .ToListAsync();
        }

        public async Task<IEnumerable<DomainProduct>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
        }

        #endregion

        #region Paginación

        public async Task<(IEnumerable<DomainProduct> Products, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string? search = null, int? categoryId = null)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        #endregion

        #region Consultas adicionales

        public async Task<IEnumerable<DomainProduct>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DomainProduct>> SearchByNameAsync(string namePattern)
        {
            return await _context.Products
                .Where(p => p.Name.Contains(namePattern))
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        #endregion
    }
}