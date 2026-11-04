using BackEntity;
using BackMappers;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackDAL
{
    public class PedidoDAL
    {
        // 1. Declaramos las variables arriba (sin asignarles valor todavía)
        private EnvioDAL envios;
        private ProductoDAL productos;
        private readonly string ConexionBS;

        public PedidoDAL() { }

        // 2. En el constructor que recibe la conexión, inicializamos todo correctamente
        public PedidoDAL(string cadenaConexion)
        {
            ConexionBS = cadenaConexion;
            envios = new EnvioDAL(ConexionBS);
            productos = new ProductoDAL(ConexionBS);
        }

        public void AgregarPedidoBS(PedidoEntity Valido)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta =
                        @"INSERT INTO PEDIDO (ID_USUARIO, ID_PRODUCTO, CANTIDAD, ID_ENVIO, MONTO_TOTAL, PRIORIDAD, FECHA_REGISTRO) 
                        VALUES (@idUsuario, @idProducto, @cantidad, @idEnvio, @montoTotal, @prioridad, @fechaRegistro)";

                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@idUsuario", Valido.Usuario.IDUsuario);
                        comando.Parameters.AddWithValue("@idProducto", Valido.Producto.IDProducto);
                        comando.Parameters.AddWithValue("@cantidad", Valido.Cantidad);
                        comando.Parameters.AddWithValue("@idEnvio", Valido.Envio.IDEnvio);
                        comando.Parameters.AddWithValue("@montoTotal", Valido.MontoTotal);
                        comando.Parameters.AddWithValue("@prioridad", Valido.Prioridad);
                        comando.Parameters.AddWithValue("@fechaRegistro", Valido.FechaRegistro);

                        comando.ExecuteNonQuery();
                    }
                }
                
            }
            catch
            {
                throw;
            }
        }

        public List<PedidoEntity> ObtemerPedidosBS()
        {
            try
            {
                List<PedidoEntity> Pedidos = new List<PedidoEntity>();
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta = "SELECT * FROM PEDIDO";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                PedidoEntity nuevo = new PedidoEntity();
                                EnvioEntity envio = envios.BuscarIDenBS(Convert.ToInt32(lector["ID_ENVIO"]));
                                ProductoEntity producto = productos.ProductoByIDenBS(Convert.ToInt32(lector["ID_PRODUCTO"]));
                                nuevo = PedidoMapper.map(producto, envio, lector);
                                Pedidos.Add(nuevo);
                            }
                        }
                    }
                }
                return Pedidos;
            }
            catch
            {
                throw;
            }
        }

        public PedidoEntity GetByIDBS(int P)
        {
            try
            {
                PedidoEntity buscado = null;
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta = "SELECT * FROM PEDIDO WHERE ID_PEDIDO = @id";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@id", P);
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                ProductoEntity producto = productos.ProductoByIDenBS(Convert.ToInt32(lector["ID_PRODUCTO"]));
                                EnvioEntity envio = envios.BuscarIDenBS(Convert.ToInt32(lector["ID_ENVIO"]));
                                buscado = PedidoMapper.map(producto, envio, lector);
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

        public void ModificarPedidoBS(PedidoEntity pedido)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta = "UPDATE PEDIDO SET ID_PRODUCTO = @producto, CANTIDAD = @cantidad, ID_ENVIO = @envio, MONTO_TOTAL = @monto ,PRIORIDAD = @prioridad, FECHA_REGISTRO = @fecha WHERE ID_PEDIDO = @id";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@id", pedido.IDpedido);
                        comando.Parameters.AddWithValue("@producto", pedido.Producto.IDProducto);
                        comando.Parameters.AddWithValue("@cantidad", pedido.Cantidad);
                        comando.Parameters.AddWithValue("@envio", pedido.Envio.IDEnvio);
                        comando.Parameters.AddWithValue("@monto", pedido.MontoTotal);
                        comando.Parameters.AddWithValue("@prioridad", pedido.Prioridad);
                        comando.Parameters.AddWithValue("@fecha", pedido.FechaRegistro);
                        comando.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public List<PedidoEntity> ObtenerPedidosPorUsuario(int idUsuario)
        {
            List<PedidoEntity> listaPedidos = new List<PedidoEntity>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta = @"SELECT P.ID_PEDIDO, P.CANTIDAD, P.MONTO_TOTAL, P.PRIORIDAD, P.FECHA_REGISTRO,
                                       PR.ID_PRODUCTO, PR.TIPO_PRODUCTO, PR.DESCRIPCION, PR.PRECIO,
                                       E.ID_ENVIO, E.ZONA, E.PRECIO AS PRECIO_ENVIO
                                FROM PEDIDO P
                                INNER JOIN PRODUCTO PR ON P.ID_PRODUCTO = PR.ID_PRODUCTO
                                INNER JOIN ENVIO E ON P.ID_ENVIO = E.ID_ENVIO
                                WHERE P.ID_USUARIO = @idUsuario";

                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@idUsuario", idUsuario);
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                PedidoEntity pedido = new PedidoEntity
                                {
                                    IDpedido = Convert.ToInt32(lector["ID_PEDIDO"]),
                                    Cantidad = Convert.ToInt32(lector["CANTIDAD"]),
                                    MontoTotal = Convert.ToDecimal(lector["MONTO_TOTAL"]),
                                    Prioridad = lector["PRIORIDAD"].ToString(),
                                    FechaRegistro = Convert.ToDateTime(lector["FECHA_REGISTRO"]),
                                    Producto = new ProductoEntity
                                    {
                                        IDProducto = Convert.ToInt32(lector["ID_PRODUCTO"]),
                                        ProductoTipo = lector["TIPO_PRODUCTO"].ToString(),
                                        Descripcion = lector["DESCRIPCION"].ToString(),
                                        Precio = Convert.ToDecimal(lector["PRECIO"])
                                    },
                                    Envio = new EnvioEntity
                                    {
                                        IDEnvio = Convert.ToInt32(lector["ID_ENVIO"]),
                                        Zona = lector["ZONA"].ToString()
                                    }
                                };
                                listaPedidos.Add(pedido);
                            }
                        }
                    }
                }
                return listaPedidos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los pedidos del usuario", ex);
            }
        }

        public void eliminarPedidoBS(int ID)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(ConexionBS))
                {
                    conexion.Open();
                    string consulta = "DELETE FROM PEDIDO WHERE ID_PEDIDO = @id";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@id", ID);
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