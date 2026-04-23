using ApiEcommerce.Application.Interfaces;
using ApiEcommerce.Application.DTOs;
using DomainCategory = ApiEcommerce.Domain.Entities.Category;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Infrastructure.Persistence
{
    /// <summary>
    /// Implementación del repositorio de categorías en Infrastructure
    /// ✅ Implementa la interfaz definida en Application
    /// ✅ Usa Entity Framework para persistencia
    /// ✅ Trabaja directamente con entidades Domain
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Consultas básicas

        public async Task<IEnumerable<DomainCategory>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<DomainCategory?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<DomainCategory?> GetByNameAsync(string name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        #endregion

        #region Operaciones de escritura

        public async Task<DomainCategory> CreateAsync(CreateCategoryDto createCategoryDto)
        {
            // 🎯 Usar factory method del dominio
            var category = DomainCategory.Create(createCategoryDto.Name);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<DomainCategory> CreateAsync(DomainCategory category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<DomainCategory> UpdateAsync(DomainCategory category)
        {
            var existingCategory = await _context.Categories.FindAsync(category.Id);
            if (existingCategory == null)
                throw new ArgumentException($"Categoría con ID {category.Id} no encontrada");

            // Actualizar usando EF Core change tracking
            existingCategory.Name = category.Name;
            
            await _context.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Consultas de negocio

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _context.Categories.AnyAsync(c => c.Name == name);
        }

        public async Task<bool> NameExistsAsync(string name, int excludeCategoryId)
        {
            return await _context.Categories.AnyAsync(c => c.Name == name && c.Id != excludeCategoryId);
        }

        public async Task<bool> HasProductsAsync(int categoryId)
        {
            return await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
        }

        #endregion

        #region Estadísticas

        public async Task<int> GetProductCountByCategoryAsync(int categoryId)
        {
            return await _context.Products.CountAsync(p => p.CategoryId == categoryId);
        }

        public async Task<IEnumerable<(DomainCategory Category, int ProductCount)>> GetCategoriesWithProductCountAsync()
        {
            var result = await _context.Categories
                .Select(c => new { Category = c, ProductCount = _context.Products.Count(p => p.CategoryId == c.Id) })
                .ToListAsync();

            return result.Select(x => (x.Category, x.ProductCount));
        }

        #endregion

        #region Consultas adicionales

        public async Task<IEnumerable<DomainCategory>> GetCategoriesCreatedAfterAsync(DateTime date)
        {
            return await _context.Categories
                .Where(c => c.CreationDate > date)
                .ToListAsync();
        }

        public async Task<IEnumerable<DomainCategory>> SearchByNameAsync(string namePattern)
        {
            return await _context.Categories
                .Where(c => c.Name.Contains(namePattern))
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Categories.CountAsync();
        }

        public async Task<IEnumerable<DomainCategory>> GetPagedAsync(int skip, int take)
        {
            return await _context.Categories
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        #endregion
    }
}