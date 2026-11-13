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
        // 1. La variable de conexión
        private readonly string ConexionBS;

        // 2. Constructor vacío por si se llama sin parámetros
        public ProductoDAL() { }

        // 3. El constructor que recibe la cadena de conexión (¡clave!)
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

        public void Actualizar(ProductoEntity producto)
        {



            using (SqlConnection conexion = new SqlConnection(ConexionBS))
            {

                string query = "UPDATE PRODUCTO SET DESCRIPCION = @Descripcion, TIPO_PRODUCTO = @ProductoTipo, PRECIO = @Precio WHERE ID_PRODUCTO = @IDProducto";
                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@IDProducto", producto.IDProducto);
                    comando.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    comando.Parameters.AddWithValue("@ProductoTipo", producto.ProductoTipo);
                    comando.Parameters.AddWithValue("@Precio", producto.Precio);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }
            
        
            
        

        // ELIMINAR PRODUCTO
        public void Eliminar(int idProducto)
        {
            using (SqlConnection conexion = new SqlConnection(ConexionBS))
            {
                string query = "DELETE FROM PRODUCTO WHERE ID_PRODUCTO = @IDProducto";
                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@IDProducto", idProducto);
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
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

