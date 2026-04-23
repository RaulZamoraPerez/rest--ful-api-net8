using ApiEcommerce.Application.DTOs;
using ApiEcommerce.Application.Interfaces;
using DomainProduct = ApiEcommerce.Domain.Entities.Product;

namespace ApiEcommerce.Application.UseCases.Products
{
    /// <summary>
    /// Caso de uso: Obtener producto por ID
    /// ✅ Incluye información de categoría
    /// ✅ Valida que el producto existe
    /// </summary>
    public class GetProductByIdUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public GetProductByIdUseCase(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ProductDto> ExecuteAsync(int productId)
        {
            if (productId <= 0)
                throw new ArgumentException("El ID del producto debe ser mayor a 0");

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new ArgumentException($"No se encontró el producto con ID {productId}");

            // Obtener información de la categoría
            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);

            return new ProductDto
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
        }
    }

    /// <summary>
    /// Caso de uso: Actualizar producto completo
    /// ✅ Valida que el producto existe
    /// ✅ Valida que el SKU no esté duplicado
    /// ✅ Valida que la categoría existe
    /// </summary>
    public class UpdateProductUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateProductUseCase(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ProductDto> ExecuteAsync(int productId, UpdateProductDto updateDto)
        {
            // 🎯 VALIDAR QUE EL PRODUCTO EXISTE
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new ArgumentException($"No se encontró el producto con ID {productId}");

            // 🎯 VALIDAR CATEGORÍA
            var categoryExists = await _categoryRepository.ExistsAsync(updateDto.CategoryId);
            if (!categoryExists)
                throw new ArgumentException($"La categoría con ID {updateDto.CategoryId} no existe");

            // 🎯 VALIDAR SKU ÚNICO
            var skuExists = await _productRepository.SkuExistsAsync(updateDto.SKU, productId);
            if (skuExists)
                throw new ArgumentException($"Ya existe otro producto con SKU '{updateDto.SKU}'");

            // 🎯 APLICAR ACTUALIZACIONES usando métodos de dominio
            product.UpdateInfo(updateDto.Name, updateDto.Description, updateDto.SKU);
            product.UpdatePrice(updateDto.Price);
            product.UpdateStock(updateDto.Stock);
            product.UpdateImages(updateDto.ImgUrl, updateDto.ImgUrlLocal);

            // 🎯 PERSISTIR
            var updatedProduct = await _productRepository.UpdateAsync(product);

            // 🎯 RETORNAR DTO CON CATEGORÍA
            var category = await _categoryRepository.GetByIdAsync(updatedProduct.CategoryId);

            return new ProductDto
            {
                ProductId = updatedProduct.ProductId,
                Name = updatedProduct.Name,
                Description = updatedProduct.Description,
                Price = updatedProduct.Price,
                ImgUrl = updatedProduct.ImgUrl,
                ImgUrlLocal = updatedProduct.ImgUrlLocal,
                SKU = updatedProduct.SKU,
                Stock = updatedProduct.Stock,
                CreationDate = updatedProduct.CreationDate,
                UpdateDate = updatedProduct.UpdateDate,
                CategoryId = updatedProduct.CategoryId,
                CategoryName = category?.Name ?? "Sin categoría"
            };
        }
    }

    /// <summary>
    /// Caso de uso: Eliminar producto
    /// ✅ Valida que el producto existe
    /// ✅ Podrían agregarse validaciones de negocio (ej: no eliminar si tiene órdenes)
    /// </summary>
    public class DeleteProductUseCase
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool> ExecuteAsync(int productId)
        {
            if (productId <= 0)
                throw new ArgumentException("El ID del producto debe ser mayor a 0");

            var productExists = await _productRepository.ExistsAsync(productId);
            if (!productExists)
                throw new ArgumentException($"No se encontró el producto con ID {productId}");

            // 🎯 AQUÍ PODRÍAN IR VALIDACIONES ADICIONALES
            // Por ejemplo: verificar que no tenga órdenes pendientes
            
            return await _productRepository.DeleteAsync(productId);
        }
    }

    /// <summary>
    /// Caso de uso: Obtener productos con paginación
    /// ✅ Incluye filtros de búsqueda y categoría
    /// ✅ Retorna información de paginación
    /// </summary>
    public class GetProductsPagedUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public GetProductsPagedUseCase(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<(IEnumerable<ProductDto> Products, int TotalCount, int TotalPages)> ExecuteAsync(
            int page, int pageSize, string? search = null, int? categoryId = null)
        {
            // 🎯 VALIDACIONES
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Límite máximo

            // 🎯 VALIDAR CATEGORÍA SI SE ESPECIFICA
            if (categoryId.HasValue)
            {
                var categoryExists = await _categoryRepository.ExistsAsync(categoryId.Value);
                if (!categoryExists)
                    throw new ArgumentException($"La categoría con ID {categoryId} no existe");
            }

            // 🎯 OBTENER DATOS PAGINADOS
            var (products, totalCount) = await _productRepository.GetPagedAsync(page, pageSize, search, categoryId);

            // 🎯 CONVERTIR A DTOs CON INFORMACIÓN DE CATEGORÍA
            var productDtos = new List<ProductDto>();
            
            foreach (var product in products)
            {
                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                
                productDtos.Add(new ProductDto
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
                });
            }

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return (productDtos, totalCount, totalPages);
        }
    }
}