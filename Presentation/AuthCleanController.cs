using ApiEcommerce.Application.DTOs;
using ApiEcommerce.Application.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Presentation
{
    /// <summary>
    /// Controller de autenticación con Clean Architecture
    /// ✅ Maneja login, registro y gestión de roles
    /// ✅ Usa casos de uso para toda la lógica
    /// </summary>
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthCleanController : ControllerBase
    {
        private readonly LoginUseCase _loginUseCase;
        private readonly RegisterUseCase _registerUseCase;

        public AuthCleanController(
            LoginUseCase loginUseCase,
            RegisterUseCase registerUseCase)
        {
            _loginUseCase = loginUseCase;
            _registerUseCase = registerUseCase;
        }

        /// <summary>
        /// 🔐 Login de usuario
        /// ✅ Valida credenciales y retorna JWT token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (loginDto == null)
                return BadRequest("Los datos de login son requeridos");

            try
            {
                var response = await _loginUseCase.ExecuteAsync(loginDto);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// 📝 Registro de nuevo usuario
        /// ✅ Valida datos y crea usuario
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
        {
            if (registerDto == null)
                return BadRequest("Los datos de registro son requeridos");

            try
            {
                var user = await _registerUseCase.ExecuteAsync(registerDto);
                return CreatedAtAction(nameof(Login), user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// 👤 Obtener información del usuario autenticado
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var userEmail = User.FindFirst("email")?.Value;
                if (string.IsNullOrEmpty(userEmail))
                    return Unauthorized("Token inválido");

                var response = new
                {
                    Id = User.FindFirst("sub")?.Value,
                    Name = User.FindFirst("name")?.Value,
                    Email = userEmail,
                    Roles = User.FindAll("role").Select(c => c.Value).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}