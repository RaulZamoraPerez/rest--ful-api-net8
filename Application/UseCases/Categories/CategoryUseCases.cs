using ApiEcommerce.Application.DTOs;
using ApiEcommerce.Application.Interfaces;
using AutoMapper;
using DomainCategory = ApiEcommerce.Domain.Entities.Category;

namespace ApiEcommerce.Application.UseCases.Categories
{
    /// <summary>
    /// ⚡ UseCase 1: Obtener todas las categorías
    /// Responsabilidad: Aplicar reglas de negocio para consultas
    /// </summary>
    public class GetAllCategoriesUseCase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetAllCategoriesUseCase(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> ExecuteAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
    }

    /// <summary>
    /// ⚡ UseCase 2: Crear nueva categoría
    /// Responsabilidad: Validar y aplicar reglas de creación
    /// </summary>
    public class CreateCategoryUseCase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CreateCategoryUseCase(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDto> ExecuteAsync(CreateCategoryDto createDto)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(createDto.Name))
                throw new ArgumentException("El nombre de la categoría es requerido");

            //  Verificar que no existe el nombre
            if (await _categoryRepository.NameExistsAsync(createDto.Name.Trim()))
                throw new InvalidOperationException($"Ya existe una categoría con el nombre '{createDto.Name}'");

            //  Usar Factory Method de dominio
            var category = DomainCategory.Create(createDto.Name.Trim());

            //  Persistir usando repositorio
            var createdCategory = await _categoryRepository.CreateAsync(category);

            return _mapper.Map<CategoryDto>(createdCategory);
        }
    }

    /// <summary>
    ///  UseCase 3: Eliminar categoría
    /// Responsabilidad: Validar reglas de eliminación
    /// </summary>
    public class DeleteCategoryUseCase
    {
        private readonly ICategoryRepository _categoryRepository;

        public DeleteCategoryUseCase(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<bool> ExecuteAsync(int id)
        {
            //  Validar que existe
            if (!await _categoryRepository.ExistsAsync(id))
                throw new ArgumentException($"La categoría con ID {id} no existe");

            return await _categoryRepository.DeleteAsync(id);
        }
    }
}