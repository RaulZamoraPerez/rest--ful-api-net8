namespace ApiEcommerce.Application.DTOs
{
    /// <summary>
    /// DTO para mostrar información básica de usuario
    /// </summary>
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para crear usuario
    /// </summary>
    public class CreateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Role { get; set; }
    }

    /// <summary>
    /// DTO para login de usuario
    /// </summary>
    public class UserLoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para registro de usuario
    /// </summary>
    public class UserRegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; }
    }

    /// <summary>
    /// DTO para respuesta de login exitoso
    /// </summary>
    public class UserLoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDataDto User { get; set; } = new();
    }

    /// <summary>
    /// DTO para información completa de usuario autenticado
    /// </summary>
    public class UserDataDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}