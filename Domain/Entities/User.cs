using System;

namespace ApiEcommerce.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio User (NO Identity) - Para datos de negocio
    /// ✅ NO depende de Identity Framework
    /// ✅ Solo para lógica de negocio de usuarios
    /// ✅ Separada de autenticación/autorización
    /// </summary>
    public class User
    {
        private User() { }

        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string UserName { get; private set; } = string.Empty;
        public string? Role { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime? UpdateDate { get; private set; }
        public bool IsActive { get; private set; }

        // 🎯 FACTORY METHOD - Para crear NUEVOS usuarios
        public static User Create(string name, string userName, string? role = null)
        {
            // 🔒 VALIDACIONES DE DOMINIO
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre es requerido", nameof(name));
            
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("El nombre de usuario es requerido", nameof(userName));
            
            if (userName.Length < 3)
                throw new ArgumentException("El nombre de usuario debe tener al menos 3 caracteres", nameof(userName));

            return new User
            {
                Name = name.Trim(),
                UserName = userName.Trim().ToLower(),
                Role = role?.Trim(),
                CreationDate = DateTime.Now,
                IsActive = true
            };
        }

        // 🔄 FACTORY METHOD - Para reconstruir usuarios EXISTENTES desde BD
        public static User Reconstruct(int id, string name, string userName, string? role, 
            DateTime creationDate, DateTime? updateDate = null, bool isActive = true)
        {
            if (id <= 0)
                throw new ArgumentException("ID debe ser mayor a 0", nameof(id));
            
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name no puede estar vacío", nameof(name));
            
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("UserName no puede estar vacío", nameof(userName));

            return new User
            {
                Id = id,
                Name = name,
                UserName = userName,
                Role = role,
                CreationDate = creationDate,
                UpdateDate = updateDate,
                IsActive = isActive
            };
        }

        // 🎯 MÉTODOS DE DOMINIO - Comportamientos del negocio
        public void UpdateInfo(string name, string? role = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre es requerido", nameof(name));

            Name = name.Trim();
            Role = role?.Trim();
            UpdateDate = DateTime.Now;
        }

        public void ChangeRole(string newRole)
        {
            if (string.IsNullOrWhiteSpace(newRole))
                throw new ArgumentException("El rol es requerido", nameof(newRole));

            Role = newRole.Trim();
            UpdateDate = DateTime.Now;
        }

        public void Activate()
        {
            IsActive = true;
            UpdateDate = DateTime.Now;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateDate = DateTime.Now;
        }

        public bool CanPerformAction(string requiredRole)
        {
            return IsActive && (Role?.Equals(requiredRole, StringComparison.OrdinalIgnoreCase) ?? false);
        }
    }
}