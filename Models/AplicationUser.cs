using Microsoft.AspNetCore.Identity;

namespace ApiEcommerce.Models;

//  IDENTIDAD - Usuario para autenticación (ASP.NET Identity)
// Este es el único modelo legacy que mantenemos porque es requerido por Identity
public class AplicationUser : IdentityUser
{
    // Propiedades adicionales para el usuario
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    //  Propiedad Name requerida por JWT
    public string? Name { get; set; }
}