using BackEntity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackDAL
{
    public class UsuarioDAL
    {
        private  readonly string  ConexionBS;
        public UsuarioDAL(){ }
        public UsuarioDAL(string conexion)
        {
            ConexionBS = conexion;
        }
        public void Registrar(UsuarioEntity Nuevo)
        {
            try
            {
                using(SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta = "INSERT INTO USUARIO (NOMBRE_USUARIO, APELLIDO_USUARIO, CONTRASEÑA_USUARIO, EMAIL_USUARIO) VALUES (@nombre, @apellido, @contraseña, @email)";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@nombre", Nuevo.Nombre);
                        comando.Parameters.AddWithValue("@apellido", Nuevo.Apellido);
                        comando.Parameters.AddWithValue("@contraseña", Nuevo.Contraseña);
                        comando.Parameters.AddWithValue("@email", Nuevo.Email);
                        comando.ExecuteNonQuery();
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar usuario", ex);
            }
        }
        public UsuarioEntity validarUsuario(UsuarioEntity Nuevo)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta = "SELECT ID_USUARIO, NOMBRE_USUARIO, APELLIDO_USUARIO, EMAIL_USUARIO FROM USUARIO WHERE EMAIL_USUARIO = @email AND CONTRASEÑA_USUARIO = @contraseña";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@email", Nuevo.Email);
                        comando.Parameters.AddWithValue("@contraseña", Nuevo.Contraseña);

                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                UsuarioEntity usuarioEncontrado = new UsuarioEntity
                                {
                                    IDUsuario = Convert.ToInt32(lector["ID_USUARIO"]),
                                    Nombre = lector["NOMBRE_USUARIO"].ToString(),
                                    Apellido = lector["APELLIDO_USUARIO"].ToString(),
                                    Email = lector["EMAIL_USUARIO"].ToString()
                                };
                                return usuarioEncontrado; // Devolvemos todos los datos
                            }
                            else
                            {
                                throw new Exception("Usuario o contraseña incorrectos");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar usuario", ex);
            }
        }
    }
}
