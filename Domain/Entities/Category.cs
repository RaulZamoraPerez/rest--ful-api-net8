using System;

namespace ApiEcommerce.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio Category - Núcleo del negocio
    /// ✅ NO depende de EF, tecnologías o frameworks externos
    /// ✅ Contiene lógica de negocio pura
    /// ✅ Representa el concepto real del negocio
    /// </summary>
    public class Category
    {
        // Constructor privado para garantizar que siempre se use el factory method
        private Category() { }

        public int Id { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public DateTime CreationDate { get; internal set; }

        //  FACTORY METHOD - Para crear  categorías
        public static Category Create(string name)
        {
            // 🔒 VALIDACIONES DE DOMINIO
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre de la categoría es requerido", nameof(name));
            
            if (name.Length > 100)
                throw new ArgumentException("El nombre de la categoría no puede exceder 100 caracteres", nameof(name));

            return new Category
            {
                Name = name.Trim(),
                CreationDate = DateTime.Now
            };
        }

        // FACTORY METHOD - Para reconstruir categorías EXISTENTES desde BD
        public static Category Reconstruct(int id, string name, DateTime creationDate)
        {
            //  Este método es para Infrastructure, no valida porque 
            // asumimos que los datos de BD ya fueron validados al crear
            
            if (id <= 0)
                throw new ArgumentException("ID debe ser mayor a 0", nameof(id));
                
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nombre no puede estar vacío", nameof(name));

            return new Category
            {
                Id = id,
                Name = name,
                CreationDate = creationDate
            };
        }

        //  MÉTODOS DE DOMINIO - Comportamientos del negocio
        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("El nombre de la categoría es requerido", nameof(newName));
            
            if (newName.Length > 100)
                throw new ArgumentException("El nombre de la categoría no puede exceder 100 caracteres", nameof(newName));

            Name = newName.Trim();
        }

        // Método para verificar si el nombre es válido
        public bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Length <= 100;
        }

        // Override para comparación
        public override bool Equals(object? obj)
        {
            return obj is Category category &&
                   Id == category.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"Category: {Name} (ID: {Id})";
        }
    }
}