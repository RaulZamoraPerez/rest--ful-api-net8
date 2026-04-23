using ApiEcommerce.Application.DTOs;
using ApiEcommerce.Application.Interfaces;

namespace ApiEcommerce.Application.UseCases.Auth
{
    /// <summary>
    /// Caso de uso: Login de usuario
    /// ✅ Encapsula la lógica de autenticación
    /// ✅ Valida credenciales y genera token
    /// </summary>
    public class LoginUseCase
    {
        private readonly IIdentityService _identityService;

        public LoginUseCase(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<UserLoginResponseDto> ExecuteAsync(UserLoginDto loginDto)
        {
            // 🎯 VALIDACIONES DE ENTRADA
            if (string.IsNullOrWhiteSpace(loginDto.Email))
                throw new ArgumentException("El email es requerido");

            if (string.IsNullOrWhiteSpace(loginDto.Password))
                throw new ArgumentException("La contraseña es requerida");

            // 🎯 INTENTAR LOGIN
            var (success, message, token) = await _identityService.LoginAsync(loginDto.Email, loginDto.Password);

            if (!success)
                throw new UnauthorizedAccessException(message);

            // 🎯 OBTENER INFORMACIÓN ADICIONAL DEL USUARIO
            var user = await _identityService.GetUserByEmailAsync(loginDto.Email);
            var roles = await _identityService.GetUserRolesAsync(loginDto.Email);

            return new UserLoginResponseDto
            {
                Token = token!,
                User = new UserDataDto
                {
                    Id = user!.Id,
                    Name = user.Name ?? "",
                    Email = user.Email ?? "",
                    Roles = roles.ToList()
                }
            };
        }
    }

    /// <summary>
    /// Caso de uso: Registro de usuario
    /// ✅ Valida datos y crea usuario en Identity
    /// </summary>
    public class RegisterUseCase
    {
        private readonly IIdentityService _identityService;

        public RegisterUseCase(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<UserDto> ExecuteAsync(UserRegisterDto registerDto)
        {
            // 🎯 VALIDACIONES DE ENTRADA
            if (string.IsNullOrWhiteSpace(registerDto.Email))
                throw new ArgumentException("El email es requerido");

            if (string.IsNullOrWhiteSpace(registerDto.Password))
                throw new ArgumentException("La contraseña es requerida");

            if (string.IsNullOrWhiteSpace(registerDto.Name))
                throw new ArgumentException("El nombre es requerido");

            if (registerDto.Password.Length < 6)
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres");

            // 🎯 CREAR USUARIO
            var (success, message) = await _identityService.RegisterAsync(
                registerDto.Email, 
                registerDto.Password, 
                registerDto.Name, 
                registerDto.Role ?? "User"
            );

            if (!success)
                throw new InvalidOperationException(message);

            // 🎯 RETORNAR INFORMACIÓN DEL USUARIO CREADO
            var user = await _identityService.GetUserByEmailAsync(registerDto.Email);

            return new UserDto
            {
                Id = user!.Id,
                Name = user.Name ?? "",
                Email = user.Email ?? "",
                UserName = user.UserName ?? ""
            };
        }
    }
}