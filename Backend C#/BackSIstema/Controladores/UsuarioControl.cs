using BackBLL;
using BackDAL;
using BackEntity;
using Microsoft.AspNetCore.Mvc;

namespace BackSIstema.Controladores
{
    [Route("api/[controller]")] // Quedaría como api/usuariocontrol
    [ApiController]
    public class UsuarioControl : ControllerBase
    {
        private readonly UsuarioBLL BLL;

        public UsuarioControl(UsuarioBLL bll)
        {
            BLL = bll;
        }

        [HttpPost("Registrar")]
        public IActionResult Registrar([FromBody] UsuarioEntity Nuevo)
        {
            try
            {
                BLL.RegistrarUsuario(Nuevo);
                return Ok(new { mensaje = "Usuario registrado correctamente." });
            }
            catch (Exception ex) // Capturamos la excepción general por si viene de la BLL/DAL
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("Validar")]
        public IActionResult Validar([FromBody] LoginEntity credenciales)
        {
            try
            {
                // La BLL ejecuta la regla de negocio limpia y busca el usuario
                UsuarioEntity usuarioLogueado = BLL.ValidarUsuario(credenciales);

                if (usuarioLogueado == null)
                {
                    return Unauthorized(new { mensaje = "Credenciales inválidas." });
                }

                // Devolvemos el JSON mapeado con el formato exacto que exige el app.js del cliente
                return Ok(new
                {
                    mensaje = "Inicio de sesión exitoso.",
                    usuario = new
                    {
                        idUsuario = usuarioLogueado.IDUsuario, // <- Esto es lo que busca desesperadamente el JS
                        nombre = usuarioLogueado.Nombre,
                        apellido = usuarioLogueado.Apellido,
                        email = usuarioLogueado.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginEntity login)
        {
            try
            {
                // 1. Ejecutamos tu método de la BLL.
                // Recordá que este método ya evalúa si es el admin (ID -1) o si va a la base de datos.
                UsuarioEntity usuario = BLL.ValidarUsuario(login);

                // 2. Si la BLL devolvió un usuario, las credenciales son correctas
                if (usuario != null)
                {
                    // Devolvemos los datos del usuario en formato JSON (incluyendo su ID, Nombre y Email)
                    return Ok(usuario);
                }

                // 3. Si es null, el usuario o contraseña no coinciden
                return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });
            }
            catch (Exception ex)
            {
                // Manejo de errores generales de la base de datos o sistema
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}