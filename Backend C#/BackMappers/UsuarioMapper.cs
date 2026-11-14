using System;
using Microsoft.Data.SqlClient; // O System.Data.SqlClient según tu proyecto
using BackEntity;

public static class UsuarioMapper 
{
    public static UsuarioEntity map(SqlDataReader lector)
    {
        UsuarioEntity usuario = new UsuarioEntity();

        usuario.IDUsuario = Convert.ToInt32(lector["ID_USUARIO"]);

            usuario.Nombre = lector["NOMBRE_USUARIO"].ToString();

            usuario.Apellido = lector["APELLIDO_USUARIO"].ToString();

            usuario.Email = lector["EMAIL_USUARIO"].ToString();

        return usuario;
    }

}