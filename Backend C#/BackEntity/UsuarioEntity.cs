using System;
using System.Collections.Generic;
using System.Text;

namespace BackEntity
{
    public class UsuarioEntity
    {
        public int IDUsuario { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Contraseña { get; set; }  
        public string? Email { get; set; }
    }
}
