using ApiEcommerce.Application.DTOs;
using ApiEcommerce.Application.UseCases.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Presentation
{
    /// <summary>
    /// Controller de categorías con Clean Architecture
    ///  Solo maneja HTTP (entrada/salida)
    ///  Delega toda la lógica a UseCases
    ///  No conoce de base de datos ni lógica de negocio
    /// </summary>
    [Route("api/v{version:apiVersion}/categories")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize] // Solo requiere estar autenticado, cualquier rol
    public class CategoriesCleanController : ControllerBase
    {
        private readonly GetAllCategoriesUseCase _getAllCategoriesUseCase;
        private readonly CreateCategoryUseCase _createCategoryUseCase;
        private readonly DeleteCategoryUseCase _deleteCategoryUseCase;

        public CategoriesCleanController(
            GetAllCategoriesUseCase getAllCategoriesUseCase,
            CreateCategoryUseCase createCategoryUseCase,
            DeleteCategoryUseCase deleteCategoryUseCase)
        {
            _getAllCategoriesUseCase = getAllCategoriesUseCase;
            _createCategoryUseCase = createCategoryUseCase;
            _deleteCategoryUseCase = deleteCategoryUseCase;
        }

        /// <summary>
        ///  Obtener todas las categorías
        ///  Solo maneja HTTP - La lógica está en el UseCase
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // permite acceso sin autenticación
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                //  DELEGAMOS TODO al UseCase
                var categories = await _getAllCategoriesUseCase.ExecuteAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        ///  Crear nueva categoría
        ///  Solo valida HTTP - Las validaciones de negocio están en el UseCase
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            //  VALIDACIon
            if (createCategoryDto == null)
                return BadRequest("Los datos de la categoría son requeridos");

            try
            {
                //  DELEGAMOS TODO al UseCase
                var createdCategory = await _createCategoryUseCase.ExecuteAsync(createCategoryDto);
                
                //  Retornamos 201 con la ubicación del recurso creado
                return CreatedAtAction(
                    nameof(GetCategory), 
                    new { id = createdCategory.Id }, 
                    createdCategory
                );
            }
            catch (ArgumentException ex)
            {
                //  Errores de validación de negocio → BadRequest
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                //  Otros errores → Internal Server Error
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        ///  Obtener categoría por ID
        ///  Implementación simple para completar el CRUD
        /// </summary>
        [HttpGet("{id:int}", Name = "GetCategory")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCategory(int id)
        {
            //  VALIDACIÓN HTTP BÁSICA
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            try
            {
               //  Aquí deberíamos tener un GetCategoryByIdUseCase
                // Por simplicidad, lo implementamos directamente
                // TODO: Crear GetCategoryByIdUseCase
                
                return NotFound("Use Case no implementado aún");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        ///  Eliminar categoría
        ///  Maneja las validaciones de negocio en el UseCase
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)] // Para cuando tiene productos
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            //  VALIDACIÓN HTTP BÁSICA
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            try
            {
                //  DELEGAMOS TODO al UseCase
                var deleted = await _deleteCategoryUseCase.ExecuteAsync(id);
                
                if (deleted)
                    return Ok("Categoría eliminada exitosamente");
                else
                    return NotFound("Categoría no encontrada");
            }
            catch (ArgumentException ex)
            {
                //  Categoría no encontrada
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                //  No se puede eliminar (ej: tiene productos)
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                //  Otros errores
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}