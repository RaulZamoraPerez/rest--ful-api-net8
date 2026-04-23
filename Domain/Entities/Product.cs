using System;

namespace ApiEcommerce.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio Product - Núcleo del negocio
    /// ✅ NO depende de EF, tecnologías o frameworks externos
    /// ✅ Contiene lógica de negocio pura
    /// ✅ Representa el concepto real del negocio
    /// </summary>
    public class Product
    {
        // Constructor privado para garantizar que siempre se use el factory method
        private Product() { }

        public int ProductId { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public decimal Price { get; internal set; }
        public string? ImgUrl { get; internal set; }
        public string? ImgUrlLocal { get; internal set; }
        public string SKU { get; internal set; } = string.Empty;
        public int Stock { get; internal set; }
        public DateTime CreationDate { get; internal set; }
        public DateTime? UpdateDate { get; internal set; }
        public int CategoryId { get; internal set; }

        // 🎯 FACTORY METHOD - Para crear NUEVOS productos
        public static Product Create(string name, string description, decimal price, 
            string sku, int stock, int categoryId, string? imgUrl = null, string? imgUrlLocal = null)
        {
            // 🔒 VALIDACIONES DE DOMINIO
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del producto es requerido", nameof(name));
            
            if (price < 0)
                throw new ArgumentException("El precio no puede ser negativo", nameof(price));
            
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("El SKU es requerido", nameof(sku));
            
            if (stock < 0)
                throw new ArgumentException("El stock no puede ser negativo", nameof(stock));
            
            if (categoryId <= 0)
                throw new ArgumentException("Debe especificar una categoría válida", nameof(categoryId));

            return new Product
            {
                Name = name.Trim(),
                Description = description?.Trim() ?? string.Empty,
                Price = price,
                SKU = sku.Trim().ToUpper(),
                Stock = stock,
                CategoryId = categoryId,
                ImgUrl = imgUrl?.Trim(),
                ImgUrlLocal = imgUrlLocal?.Trim(),
                CreationDate = DateTime.Now
            };
        }

        // 🔄 FACTORY METHOD - Para reconstruir productos EXISTENTES desde BD
        public static Product Reconstruct(int productId, string name, string description, decimal price,
            string sku, int stock, int categoryId, DateTime creationDate, DateTime? updateDate = null,
            string? imgUrl = null, string? imgUrlLocal = null)
        {
            if (productId <= 0)
                throw new ArgumentException("ProductId debe ser mayor a 0", nameof(productId));
            
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name no puede estar vacío", nameof(name));

            return new Product
            {
                ProductId = productId,
                Name = name,
                Description = description,
                Price = price,
                SKU = sku,
                Stock = stock,
                CategoryId = categoryId,
                ImgUrl = imgUrl,
                ImgUrlLocal = imgUrlLocal,
                CreationDate = creationDate,
                UpdateDate = updateDate
            };
        }

        // 🎯 MÉTODOS DE DOMINIO - Comportamientos del negocio
        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentException("El precio no puede ser negativo", nameof(newPrice));
            
            Price = newPrice;
            UpdateDate = DateTime.Now;
        }

        public void UpdateStock(int newStock)
        {
            if (newStock < 0)
                throw new ArgumentException("El stock no puede ser negativo", nameof(newStock));
            
            Stock = newStock;
            UpdateDate = DateTime.Now;
        }

        public void UpdateImages(string? imgUrl, string? imgUrlLocal)
        {
            ImgUrl = imgUrl;
            ImgUrlLocal = imgUrlLocal;
            UpdateDate = DateTime.Now;
        }

        public bool IsInStock() => Stock > 0;
        
        public bool CanReduceStock(int quantity) => Stock >= quantity;

        public void ReduceStock(int quantity)
        {
            if (!CanReduceStock(quantity))
                throw new InvalidOperationException($"No hay suficiente stock. Disponible: {Stock}, Solicitado: {quantity}");
            
            Stock -= quantity;
            UpdateDate = DateTime.Now;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser positiva", nameof(quantity));
            
            Stock += quantity;
            UpdateDate = DateTime.Now;
        }

        // Método para actualizar información básica
        public void UpdateInfo(string name, string description, string sku)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del producto es requerido", nameof(name));
            
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("El SKU es requerido", nameof(sku));

            Name = name;
            Description = description;
            SKU = sku;
            UpdateDate = DateTime.Now;
        }
    }
}