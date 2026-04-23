using ApiEcommerce.Application.DTOs;
using ApiEcommerce.Application.Interfaces;

namespace ApiEcommerce.Application.UseCases.Products
{
    /// <summary>
    /// Caso de uso: Actualizar stock de un producto
    /// ✅ Ejemplo de caso de uso específico y enfocado
    /// ✅ Usa métodos de dominio para mantener consistencia
    /// </summary>
    public class UpdateProductStockUseCase
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductStockUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto> ExecuteAsync(int productId, UpdateStockDto updateStockDto)
        {
            // 🎯 OBTENER PRODUCTO
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new ArgumentException($"No se encontró el producto con ID {productId}");

            // 🎯 APLICAR LÓGICA DE NEGOCIO usando métodos de dominio
            switch (updateStockDto.Operation.ToLower())
            {
                case "set":
                    product.UpdateStock(updateStockDto.Quantity);
                    break;
                
                case "add":
                    product.IncreaseStock(updateStockDto.Quantity);
                    break;
                
                case "subtract":
                    product.ReduceStock(updateStockDto.Quantity);
                    break;
                
                default:
                    throw new ArgumentException("Operación no válida. Use 'set', 'add' o 'subtract'");
            }

            // 🎯 PERSISTIR
            var updatedProduct = await _productRepository.UpdateAsync(product);

            // 🎯 RETORNAR DTO
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
                CategoryId = updatedProduct.CategoryId
            };
        }
    }
}