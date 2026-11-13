using BackDAL;
using BackEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackBLL
{
    public class UsuarioBLL
    {
        private UsuarioDAL DAL;
        public UsuarioBLL(string conexion)
        {
            DAL = new UsuarioDAL(conexion);
        }
        public void RegistrarUsuario(UsuarioEntity Nuevo)
        {
            if (Nuevo.Nombre == null || Nuevo.Apellido == null || Nuevo.Contraseña == null || Nuevo.Email == null)
            {
                throw new BLLException("Todos los campos son obligatorios");
            }
            try
            {
                DAL.Registrar(Nuevo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar usuario", ex);
            }

        }
        public UsuarioEntity ValidarUsuario(UsuarioEntity Nuevo)
        {
            if ((Nuevo.Email == null || (Nuevo.Contraseña == null)))
            {
                throw new BLLException("Todos los campos son obligatorios");
            }
            try
            {
                // Retornamos el usuario que nos pasa la DAL
                return DAL.validarUsuario(Nuevo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar usuario", ex);
            }
        }
        public UsuarioEntity ValidarUsuario(LoginEntity login)
        {
            // 1. Verificamos si es el Administrador fijo del sistema
            if (login.Email == "admin@ecommerce.com" && login.Contraseña == "Admin1234")
            {
                return new UsuarioEntity
                {
                    IDUsuario = 999, // ID reservado para el admin
                    Nombre = "Administrador",
                    Email = login.Email
                };
            }

            // 2. Si no es el admin, adaptamos el login a un UsuarioEntity para la DAL 
            UsuarioEntity usuario = new UsuarioEntity
            {
                Email = login.Email,
                Contraseña = login.Contraseña
            };

            return DAL.validarUsuario(usuario);
        }
    }
}
