using ApiEcommerce.Application.DTOs;
using ApiEcommerce.Application.UseCases.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Presentation
{
    /// <summary>
    /// Controller de productos con Clean Architecture
    /// ✅ Solo maneja HTTP (entrada/salida)
    /// ✅ Delega toda la lógica a UseCases
    /// ✅ No conoce de base de datos ni lógica de negocio
    /// </summary>
    [Route("api/v{version:apiVersion}/products")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ProductsCleanController : ControllerBase
    {
        private readonly GetAllProductsUseCase _getAllProductsUseCase;
        private readonly GetProductByIdUseCase _getProductByIdUseCase;
        private readonly CreateProductUseCase _createProductUseCase;
        private readonly UpdateProductUseCase _updateProductUseCase;
        private readonly UpdateProductStockUseCase _updateStockUseCase;
        private readonly DeleteProductUseCase _deleteProductUseCase;
        private readonly GetProductsPagedUseCase _getProductsPagedUseCase;

        public ProductsCleanController(
            GetAllProductsUseCase getAllProductsUseCase,
            GetProductByIdUseCase getProductByIdUseCase,
            CreateProductUseCase createProductUseCase,
            UpdateProductUseCase updateProductUseCase,
            UpdateProductStockUseCase updateStockUseCase,
            DeleteProductUseCase deleteProductUseCase,
            GetProductsPagedUseCase getProductsPagedUseCase)
        {
            _getAllProductsUseCase = getAllProductsUseCase;
            _getProductByIdUseCase = getProductByIdUseCase;
            _createProductUseCase = createProductUseCase;
            _updateProductUseCase = updateProductUseCase;
            _updateStockUseCase = updateStockUseCase;
            _deleteProductUseCase = deleteProductUseCase;
            _getProductsPagedUseCase = getProductsPagedUseCase;
        }

        /// <summary>
        /// 📋 Obtener todos los productos con filtros
        /// ✅ Delegación completa al UseCase
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? search,
            [FromQuery] int? categoryId)
        {
            try
            {
                var products = await _getAllProductsUseCase.ExecuteAsync(search, categoryId);
                return Ok(products);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// 📋 Obtener productos paginados
        /// ✅ Incluye metadatos de paginación
        /// </summary>
        [HttpGet("paged")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProductsPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null)
        {
            try
            {
                var (products, totalCount, totalPages) = await _getProductsPagedUseCase
                    .ExecuteAsync(page, pageSize, search, categoryId);

                var response = new
                {
                    Data = products,
                    Pagination = new
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        HasNextPage = page < totalPages,
                        HasPreviousPage = page > 1
                    }
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// 📖 Obtener producto por ID
        /// </summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _getProductByIdUseCase.ExecuteAsync(id);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// ✨ Crear nuevo producto
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (createProductDto == null)
                return BadRequest("Los datos del producto son requeridos");

            try
            {
                var createdProduct = await _createProductUseCase.ExecuteAsync(createProductDto);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, createdProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔄 Actualizar producto completo
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null)
                return BadRequest("Los datos del producto son requeridos");

            try
            {
                var updatedProduct = await _updateProductUseCase.ExecuteAsync(id, updateProductDto);
                return Ok(updatedProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// 📦 Actualizar solo el stock
        /// </summary>
        [HttpPatch("{id:int}/stock")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto updateStockDto)
        {
            if (updateStockDto == null)
                return BadRequest("Los datos del stock son requeridos");

            try
            {
                var updatedProduct = await _updateStockUseCase.ExecuteAsync(id, updateStockDto);
                return Ok(updatedProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// 🗑️ Eliminar producto
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var deleted = await _deleteProductUseCase.ExecuteAsync(id);
                
                if (deleted)
                    return Ok("Producto eliminado exitosamente");
                else
                    return NotFound("Producto no encontrado");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}