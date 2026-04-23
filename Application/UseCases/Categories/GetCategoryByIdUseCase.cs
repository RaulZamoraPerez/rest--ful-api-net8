using ApiEcommerce.Application.Interfaces;
using ApiEcommerce.Application.DTOs;
using AutoMapper;

namespace ApiEcommerce.Application.UseCases.Categories
{
    /// <summary>
    /// Caso de uso para obtener una categoría por ID
    /// </summary>
    public class GetCategoryByIdUseCase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetCategoryByIdUseCase(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDto?> ExecuteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? _mapper.Map<CategoryDto>(category) : null;
        }
    }
}