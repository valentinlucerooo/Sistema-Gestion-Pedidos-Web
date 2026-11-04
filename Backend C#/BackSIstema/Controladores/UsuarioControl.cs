using BackBLL;
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
                UsuarioEntity usuarioTemp = new UsuarioEntity
                {
                    Email = credenciales.Email,
                    Contraseña = credenciales.Contraseña
                };

                // Recibimos el usuario completo desde la BLL
                UsuarioEntity usuarioLogueado = BLL.ValidarUsuario(usuarioTemp);

                return Ok(new
                {
                    mensaje = "Inicio de sesión exitoso.",
                    usuario = usuarioLogueado // <- Acá viajan el ID, nombre, apellido y email al frontend
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}