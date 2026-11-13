using System;
using System.Collections.Generic;
using System.Text;
using BackEntity;  
using BackMappers;
using Microsoft.Data.SqlClient;

namespace BackDAL
{
    public class EnvioDAL
    {
        // 1. Variable privada para guardar la cadena
        private readonly string ConexionBS;

        public EnvioDAL(string cadenaConexion)
        {
            ConexionBS = cadenaConexion;
        }
        public EnvioDAL()
        {
        }

        public void AgregarEnvioBS(EnvioEntity Valido)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta = "INSERT INTO ENVIO ( ZONA, PRECIO) VALUES (@zona, @precio)";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@zona", Valido.Zona);
                        comando.Parameters.AddWithValue("@precio", Valido.Precio);
                        comando.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public List<EnvioEntity> ObtenerEnviosBS()
        {
            try
            {
                List<EnvioEntity> envios = new List<EnvioEntity>();
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta = "SELECT * FROM ENVIO ";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                EnvioEntity encontrado = EnvioMapper.map(lector);
                                envios.Add(encontrado);
                            }
                        }
                    }
                }
                return envios;
            }

            catch
            {
                throw;
            }
        }
        public EnvioEntity BuscarIDenBS(int ID)
        {
            try
            {
                EnvioEntity buscado = new EnvioEntity();
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta = "SELECT * FROM ENVIO WHERE ID_ENVIO = @id";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@id", ID);
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                buscado = EnvioMapper.map(lector);
                            }
                        }

                    }
                }
                return buscado;

            }
            catch
            {
                throw;
            }
        }
        public void Actualizar(EnvioEntity envio)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    string query = "UPDATE ENVIO SET ZONA = @Zona, PRECIO = @Costo WHERE ID_ENVIO = @IDEnvio";
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IDEnvio", envio.IDEnvio);
                        comando.Parameters.AddWithValue("@Zona", envio.Zona);
                        comando.Parameters.AddWithValue("@Costo", envio.Precio);

                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public void Eliminar(int idEnvio)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    string query = "DELETE FROM ENVIO WHERE ID_ENVIO = @IDEnvio";
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IDEnvio", idEnvio);
                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}

