namespace ApiEcommerce.Application.DTOs
{
    /// <summary>
    /// DTO para mostrar información de categoría
    /// ✅ Se usa para transferir datos hacia afuera (API responses)
    /// </summary>
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        
        // Propiedades calculadas (pueden venir de casos de uso)
        public int ProductCount { get; set; }
        public string FormattedCreationDate => CreationDate.ToString("dd/MM/yyyy");
    }

    /// <summary>
    /// DTO para crear una categoría
    /// ✅ Solo contiene lo necesario para la creación
    /// </summary>
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para actualizar una categoría
    /// </summary>
    public class UpdateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para mostrar categoría con sus productos
    /// ✅ Para casos donde necesitas la categoría completa con productos
    /// </summary>
    public class CategoryWithProductsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public List<ProductDto> Products { get; set; } = new();
    }
}