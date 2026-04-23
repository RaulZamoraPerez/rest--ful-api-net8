using ApiEcommerce.Application.DTOs;
using ApiEcommerce.Application.Interfaces;
using ApiEcommerce.Domain.Entities;

namespace ApiEcommerce.Application.UseCases.Products
{
    /// <summary>
    /// Caso de uso: Obtener todos los productos con filtros
    /// ✅ Encapsula UNA responsabilidad específica
    /// ✅ Contiene la lógica de negocio pura
    /// ✅ No depende de tecnología (EF, HTTP, etc.)
    /// </summary>
    public class GetAllProductsUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public GetAllProductsUseCase(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<ProductDto>> ExecuteAsync(string? search = null, int? categoryId = null)
        {
            // 🎯 LÓGICA DE NEGOCIO
            
            // 1. Validar que la categoría existe si se especifica
            if (categoryId.HasValue)
            {
                var categoryExists = await _categoryRepository.ExistsAsync(categoryId.Value);
                if (!categoryExists)
                    throw new ArgumentException($"La categoría con ID {categoryId} no existe");
            }

            // 2. Obtener productos aplicando filtros
            var products = await _productRepository.GetProductsAsync(search, categoryId);

            // 3. Convertir a DTOs con lógica de negocio aplicada
            var result = new List<ProductDto>();

            foreach (var product in products)
            {
                // Obtener información de la categoría para el DTO
                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                
                var productDto = new ProductDto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImgUrl = product.ImgUrl,
                    ImgUrlLocal = product.ImgUrlLocal,
                    SKU = product.SKU,
                    Stock = product.Stock,
                    CreationDate = product.CreationDate,
                    UpdateDate = product.UpdateDate,
                    CategoryId = product.CategoryId,
                    CategoryName = category?.Name ?? "Sin categoría"
                };

                result.Add(productDto);
            }

            // 4. Aplicar ordenamiento por lógica de negocio (ej: primero los que tienen stock)
            return result.OrderByDescending(p => p.IsInStock)
                        .ThenBy(p => p.Name);
        }
    }
}