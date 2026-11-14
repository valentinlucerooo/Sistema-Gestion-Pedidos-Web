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
        public void AgregarEstado(int ID, string Estado)
        {
            using (SqlConnection conexion = new SqlConnection(ConexionBS))
            {
                conexion.Open();
                string consulta = "UPDATE PEDIDO SET ESTADO = @estado WHERE ID_PEDIDO = @id";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@id", ID);
                    comando.Parameters.AddWithValue("@estado", Estado);
                    comando.ExecuteNonQuery();
                }
            }

        }


        public int InsertarPedidoCompleto(PedidoEntity pedido, List<DetallePedidoEntity> detalles)
        {
            using (SqlConnection conexion = new SqlConnection(ConexionBS))
            {
                conexion.Open();
                using (SqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        // 1. Insertamos la cabecera y obtenemos el ID
                        string queryPedido = @"INSERT INTO PEDIDO (ID_USUARIO, ID_ENVIO, MONTO_TOTAL, PRIORIDAD, FECHA_REGISTRO, ENTREGA) 
                                       VALUES (@IdUsuario, @IdEnvio, @MontoTotal, @Prioridad, @FechaRegistro, @Entrega);
                                       SELECT SCOPE_IDENTITY();";

                        int idPedidoGenerado = 0;

                        using (SqlCommand cmdPedido = new SqlCommand(queryPedido, conexion, transaccion))
                        {
                            cmdPedido.Parameters.AddWithValue("@IdUsuario", pedido.Usuario.IDUsuario);
                            cmdPedido.Parameters.AddWithValue("@IdEnvio", pedido.Envio.IDEnvio);
                            cmdPedido.Parameters.AddWithValue("@MontoTotal", pedido.MontoTotal);
                            cmdPedido.Parameters.AddWithValue("@Prioridad", pedido.Prioridad);
                            cmdPedido.Parameters.AddWithValue("@FechaRegistro",pedido.FechaRegistro);
                            cmdPedido.Parameters.AddWithValue("@Entrega", pedido.Estado);

                            idPedidoGenerado = Convert.ToInt32(cmdPedido.ExecuteScalar());
                        }

                        // 2. Insertamos los detalles en el bucle
                        string queryDetalle = @"INSERT INTO DETALLE_PEDIDO (ID_PEDIDO, ID_PRODUCTO, CANTIDAD) 
                                        VALUES (@IdPedido, @IdProducto, @Cantidad)";

                        foreach (var detalle in detalles)
                        {
                            using (SqlCommand cmdDetalle = new SqlCommand(queryDetalle, conexion, transaccion))
                            {
                                cmdDetalle.Parameters.AddWithValue("@IdPedido", idPedidoGenerado);
                                cmdDetalle.Parameters.AddWithValue("@IdProducto", detalle.IDproducto);
                                cmdDetalle.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);

                                cmdDetalle.ExecuteNonQuery();
                            }
                        }
                        transaccion.Commit();
                        return idPedidoGenerado;
                    }
                    catch (Exception ex)
                    {
                        transaccion.Rollback();
                        throw new Exception("Error al registrar el pedido: " + ex.Message);
                    }
                }
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
        public PedidoEntity ObtenerPedidoPorId(int idPedido)
        {
            PedidoEntity pedido = null;

            using (SqlConnection conexion = new SqlConnection(ConexionBS))
            {
                conexion.Open();

                // PASO 1: Traemos la cabecera del pedido y los datos del envío
                string consultaPedido = @"SELECT P.ID_PEDIDO, P.ID_USUARIO, P.MONTO_TOTAL, P.PRIORIDAD, P.FECHA_REGISTRO, P.ENTREGA,
                                         E.ID_ENVIO, E.ZONA, E.PRECIO AS PRECIO_ENVIO
                                  FROM PEDIDO P
                                  INNER JOIN ENVIO E ON P.ID_ENVIO = E.ID_ENVIO
                                  WHERE P.ID_PEDIDO = @idPedido";

                using (SqlCommand comando = new SqlCommand(consultaPedido, conexion))
                {
                    comando.Parameters.AddWithValue("@idPedido", idPedido);
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            pedido = new PedidoEntity
                            {
                                IDpedido = Convert.ToInt32(lector["ID_PEDIDO"]),
                               Usuario=UsuarioMapper.map(lector),
                                MontoTotal = Convert.ToDecimal(lector["MONTO_TOTAL"]),
                                Prioridad = lector["PRIORIDAD"].ToString(),
                                FechaRegistro = Convert.ToDateTime(lector["FECHA_REGISTRO"]),
                                Estado = lector["ENTREGA"].ToString(), // O como guardes la propiedad de entrega
                                Envio = new EnvioEntity
                                {
                                    IDEnvio = Convert.ToInt32(lector["ID_ENVIO"]),
                                    Zona = lector["ZONA"].ToString(),
                                    Precio = Convert.ToDecimal(lector["PRECIO_ENVIO"])
                                },
                                // Inicializamos la lista de detalles vacía para llenarla en el siguiente paso
                                Detalles = new List<DetallePedidoEntity>()
                            };
                        }
                    }
                }

                // Si encontramos el pedido, buscamos sus detalles (los productos asociados)
                if (pedido != null)
                {
                    string consultaDetalles = @"SELECT D.ID_DETALLE, D.ID_PEDIDO, D.CANTIDAD,
                                               PR.ID_PRODUCTO, PR.TIPO_PRODUCTO, PR.DESCRIPCION, PR.PRECIO
                                        FROM DETALLE_PEDIDO D
                                        INNER JOIN PRODUCTO PR ON D.ID_PRODUCTO = PR.ID_PRODUCTO
                                        WHERE D.ID_PEDIDO = @idPedido";

                    using (SqlCommand comandoDetalle = new SqlCommand(consultaDetalles, conexion))
                    {
                        comandoDetalle.Parameters.AddWithValue("@idPedido", idPedido);
                        using (SqlDataReader lectorDetalle = comandoDetalle.ExecuteReader())
                        {
                            while (lectorDetalle.Read())
                            {
                                DetallePedidoEntity detalle = new DetallePedidoEntity
                                {
                                    IDDetallePedido = Convert.ToInt32(lectorDetalle["ID_DETALLE"]),
                                    IDpedido = Convert.ToInt32(lectorDetalle["ID_PEDIDO"]),
                                    Cantidad = Convert.ToInt32(lectorDetalle["CANTIDAD"]),
                                    // Si en tu entidad DetallePedido tenés un objeto ProductoEntity mapeado, lo cargás acá:
                                    /* Producto = new ProductoEntity { ... } */
                                    IDproducto = Convert.ToInt32(lectorDetalle["ID_PRODUCTO"])
                                };

                                pedido.Detalles.Add(detalle);
                            }
                        }
                    }
                }
            }

            return pedido;
        }
        public List<PedidoEntity> ObtenerPedidosDEUsuario(int idUsuario)
        {
            List<PedidoEntity> listaPedidos = new List<PedidoEntity>();

            using (SqlConnection conexion = new SqlConnection(ConexionBS))
            {
                conexion.Open();

                // 1. Traemos la cabecera de todos los pedidos del usuario con su envío
                string consultaPedidos = @"SELECT P.ID_PEDIDO, P.ID_USUARIO, P.MONTO_TOTAL, P.PRIORIDAD, P.FECHA_REGISTRO, P.ENTREGA, 
                                  E.ID_ENVIO, E.ZONA, E.PRECIO AS PRECIO_ENVIO, 
                                  U.NOMBRE_USUARIO, U.APELLIDO_USUARIO,U.EMAIL_USUARIO
                           FROM PEDIDO P 
                           INNER JOIN ENVIO E ON P.ID_ENVIO = E.ID_ENVIO 
                           INNER JOIN USUARIO U ON P.ID_USUARIO = U.ID_USUARIO  
                           WHERE P.ID_USUARIO = @idUsuario";

                using (SqlCommand comando = new SqlCommand(consultaPedidos, conexion))
                {
                    comando.Parameters.AddWithValue("@idUsuario", idUsuario);
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            PedidoEntity pedido = new PedidoEntity
                            {
                                IDpedido = Convert.ToInt32(lector["ID_PEDIDO"]),
                                Usuario = UsuarioMapper.map(lector),
                                MontoTotal = Convert.ToDecimal(lector["MONTO_TOTAL"]),
                                Prioridad = lector["PRIORIDAD"].ToString(),
                                FechaRegistro = Convert.ToDateTime(lector["FECHA_REGISTRO"]),
                                Estado = lector["ENTREGA"].ToString(),
                                Envio = new EnvioEntity
                                {
                                    IDEnvio = Convert.ToInt32(lector["ID_ENVIO"]),
                                    Zona = lector["ZONA"].ToString(),
                                    Precio = Convert.ToDecimal(lector["PRECIO_ENVIO"])
                                },
                                Detalles = new List<DetallePedidoEntity>()
                            };
                            listaPedidos.Add(pedido);
                        }
                    }
                }

                // 2. Recorremos cada pedido para buscar y rellenar su lista de detalles (productos)
                foreach (var pedido in listaPedidos)
                {
                    string consultaDetalles = @"SELECT D.ID_DETALLE, D.ID_PEDIDO, D.CANTIDAD,
                                               PR.ID_PRODUCTO, PR.TIPO_PRODUCTO, PR.DESCRIPCION, PR.PRECIO
                                        FROM DETALLE_PEDIDO D
                                        INNER JOIN PRODUCTO PR ON D.ID_PRODUCTO = PR.ID_PRODUCTO
                                        WHERE D.ID_PEDIDO = @idPedido";

                    using (SqlCommand comandoDetalle = new SqlCommand(consultaDetalles, conexion))
                    {
                        comandoDetalle.Parameters.AddWithValue("@idPedido", pedido.IDpedido);
                        using (SqlDataReader lectorDetalle = comandoDetalle.ExecuteReader())
                        {
                            while (lectorDetalle.Read())
                            {
                                DetallePedidoEntity detalle = new DetallePedidoEntity
                                {
                                    IDDetallePedido = Convert.ToInt32(lectorDetalle["ID_DETALLE"]),
                                    IDpedido = Convert.ToInt32(lectorDetalle["ID_PEDIDO"]),
                                    Cantidad = Convert.ToInt32(lectorDetalle["CANTIDAD"]),
                                    IDproducto = Convert.ToInt32(lectorDetalle["ID_PRODUCTO"])
                                };

                                pedido.Detalles.Add(detalle);
                            }
                        }
                    }
                }
            }

            return listaPedidos;
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