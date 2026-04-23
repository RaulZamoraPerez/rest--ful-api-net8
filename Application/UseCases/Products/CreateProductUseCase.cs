using ApiEcommerce.Application.DTOs;
using ApiEcommerce.Application.Interfaces;
using ApiEcommerce.Domain.Entities;

namespace ApiEcommerce.Application.UseCases.Products
{
    /// <summary>
    /// Caso de uso: Crear un nuevo producto
    /// ✅ Valida reglas de negocio
    /// ✅ Coordina entre repositorios
    /// ✅ Encapsula la lógica de creación
    /// </summary>
    public class CreateProductUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CreateProductUseCase(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ProductDto> ExecuteAsync(CreateProductDto createProductDto)
        {
            // 🎯 VALIDACIONES DE NEGOCIO

            // 1. Validar que la categoría existe
            var categoryExists = await _categoryRepository.ExistsAsync(createProductDto.CategoryId);
            if (!categoryExists)
                throw new ArgumentException($"La categoría con ID {createProductDto.CategoryId} no existe");

            // 2. Validar que el SKU no existe
            var skuExists = await _productRepository.SkuExistsAsync(createProductDto.SKU);
            if (skuExists)
                throw new ArgumentException($"Ya existe un producto con SKU '{createProductDto.SKU}'");

            // 3. Validaciones adicionales de negocio
            if (createProductDto.Price <= 0)
                throw new ArgumentException("El precio debe ser mayor a cero");

            // 🎯 CREAR ENTIDAD DE DOMINIO usando Factory Method
            var product = Product.Create(
                createProductDto.Name,
                createProductDto.Description,
                createProductDto.Price,
                createProductDto.SKU,
                createProductDto.Stock,
                createProductDto.CategoryId,
                createProductDto.ImgUrl,
                createProductDto.ImgUrlLocal
            );

            // 🎯 PERSISTIR
            var savedProduct = await _productRepository.CreateAsync(product);

            // 🎯 RETORNAR DTO
            var category = await _categoryRepository.GetByIdAsync(savedProduct.CategoryId);
            
            return new ProductDto
            {
                ProductId = savedProduct.ProductId,
                Name = savedProduct.Name,
                Description = savedProduct.Description,
                Price = savedProduct.Price,
                ImgUrl = savedProduct.ImgUrl,
                ImgUrlLocal = savedProduct.ImgUrlLocal,
                SKU = savedProduct.SKU,
                Stock = savedProduct.Stock,
                CreationDate = savedProduct.CreationDate,
                UpdateDate = savedProduct.UpdateDate,
                CategoryId = savedProduct.CategoryId,
                CategoryName = category?.Name ?? "Sin categoría"
            };
        }
    }
}