namespace ApiEcommerce.Application.DTOs
{
    /// <summary>
    /// DTO para mostrar información de producto
    /// ✅ Se usa para transferir datos hacia afuera (API responses)
    /// ✅ Puede tener propiedades calculadas o combinadas
    /// </summary>
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImgUrl { get; set; }
        public string? ImgUrlLocal { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int Stock { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        
        // Información de la categoría incluida
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        
        // Propiedades calculadas
        public bool IsInStock => Stock > 0;
        public string StockStatus => Stock > 10 ? "En Stock" : Stock > 0 ? "Poco Stock" : "Sin Stock";
        public string FormattedPrice => Price.ToString("C");
    }

    /// <summary>
    /// DTO para crear un producto
    /// ✅ Solo contiene lo necesario para la creación
    /// ✅ Se valida en los casos de uso
    /// </summary>
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImgUrl { get; set; }
        public string? ImgUrlLocal { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int CategoryId { get; set; }
    }

    /// <summary>
    /// DTO para actualizar un producto
    /// ✅ Puede contener propiedades opcionales
    /// ✅ Se usa en operaciones PUT/PATCH
    /// </summary>
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImgUrl { get; set; }
        public string? ImgUrlLocal { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int CategoryId { get; set; }
    }

    /// <summary>
    /// DTO para actualizar solo el stock
    /// ✅ DTO específico para operación específica
    /// </summary>
    public class UpdateStockDto
    {
        public int Quantity { get; set; }
        public string Operation { get; set; } = "set"; // "set", "add", "subtract"
    }
}