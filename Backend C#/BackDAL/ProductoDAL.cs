using BackMappers;
using System;
using System.Collections.Generic;
using System.Text;
using BackEntity;
using Microsoft.Data.SqlClient;

namespace BackDAL
{
    public class ProductoDAL
    {
        // 1. Variable privada para guardar la cadena
        private readonly string ConexionBS;

        public ProductoDAL() { }
        public ProductoDAL(string cadenaConexion)
        {
            ConexionBS = cadenaConexion;
        }
        public List<ProductoEntity> ObtenerProductosBS()
        {
            try
            {
                List<ProductoEntity> productos = new List<ProductoEntity>();
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta = "SELECT * FROM PRODUCTO";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                ProductoEntity encontrado = ProductoMapper.map(lector);
                                productos.Add(encontrado);

                            }

                        }
                    }
                }
                return productos;
            }
            catch
            {
                throw;
            }

        }
        public ProductoEntity ProductoByIDenBS(int ID)
        {
            try
            {
                ProductoEntity buscado = new ProductoEntity();
                buscado = null;
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta = "SELECT ID_PRODUCTO, TIPO_PRODUCTO, DESCRIPCION, PRECIO FROM PRODUCTO WHERE ID_PRODUCTO = @id ";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("id", ID);
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                buscado = ProductoMapper.map(lector);
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
        public void AgregarProductoBS(ProductoEntity Valido)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta = "INSERT INTO PRODUCTO ( TIPO_PRODUCTO, DESCRIPCION, PRECIO) VALUES ( @tipo, @descripcion, @precio)";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@tipo", Valido.ProductoTipo);
                        comando.Parameters.AddWithValue("@descripcion", Valido.Descripcion);
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
    }
}

