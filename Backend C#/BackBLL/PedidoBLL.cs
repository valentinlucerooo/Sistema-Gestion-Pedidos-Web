using BackDAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using System.Xml;
using BackEntity;   

namespace BackBLL
{
    public class PedidoBLL
    {
        private PedidoDAL dal;
        private EnvioDAL envioDAL;
        private ProductoDAL productoDAL = new ProductoDAL();
        public PedidoBLL(string cadenaConexion)
        {
            dal = new PedidoDAL(cadenaConexion);
            envioDAL = new EnvioDAL(cadenaConexion);
            productoDAL = new ProductoDAL(cadenaConexion);
        }
        public List<PedidoEntity> ObtenerPedidosPorUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                throw new BLLException("El ID de usuario no es válido.");
            }

            try
            {
                return dal.ObtenerPedidosPorUsuario(idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los pedidos del usuario en BLL", ex);
            }
        }
        public List<PedidoEntity> GetAll()
        {
            try
            {
                return dal.ObtemerPedidosBS();

            }
            catch
            {
                throw new BLLException("Error al obtener los pedidos.");
            }
        }
        public void ValidarBorrador(List<PedidoEntity> borrador)
        {
            try
            {
                if (borrador == null || borrador.Count == 0)
                    throw new BLLException("el borrador no puede estar vacio.");
                using (var trx = new TransactionScope())
                {
                    foreach (var pedido in borrador)
                    {
                        ValidarPedido(pedido);
                    }
                    foreach (var pedido in borrador)
                    {
                        dal.AgregarPedidoBS(pedido);
                        //aqui se guardaria el pedido en la base de datos

                    }
                    trx.Complete();
                }

            }
            catch
            {
                throw new BLLException("Error al validar el borrador.");

            }


        }
        public void ModificarPedido(PedidoEntity Modificado)
        {
            try
            {
                using (var trx = new TransactionScope())
                {
                    dal.ModificarPedidoBS(Modificado);
                    //aqui se modificaria el pedido en la base de datos
                    trx.Complete();

                }
            }
            catch
            {
                throw new BLLException("Error al modificar el pedido.");
            }

        }
        public void BorrarPedido(int ID)
        {
            try
            {
                using (var trx = new TransactionScope())
                {
                    dal.eliminarPedidoBS(ID);
                    //aqui se eliminaría el pedido en la base de datos
                    trx.Complete();

                }
            }
            catch
            {
                throw new BLLException("Error al modificar el pedido.");
            }

        }


        public void ValidarPedido(PedidoEntity Nuevo)
        {
            try
            {
                using (var trx = new TransactionScope())
                {


                    if (Nuevo.Producto == null)
                    {
                        throw new BLLException("el producto no puede estar vacio.");
                    }
                    if (Nuevo.Cantidad <= 0)
                    {
                        throw new BLLException("la cantidad debe ser mayor a cero.");
                    }
                    if (Nuevo.Envio == null)
                    {
                        throw new BLLException("debe seleccionar una zona de envio.");
                    }
                    
                    if (Nuevo.Prioridad.Length <= 3)
                    {
                        throw new BLLException("la prioridad debe tener al menos 4 caracteres");
                    }
                    Nuevo.FechaRegistro = DateTime.Now;
                    Nuevo.Producto = productoDAL.ProductoByIDenBS(Nuevo.Producto.IDProducto);
                    Nuevo.Envio = envioDAL.BuscarIDenBS(Nuevo.Envio.IDEnvio);
                    Nuevo.MontoTotal = (Nuevo.Producto.Precio * Nuevo.Cantidad) + Nuevo.Envio.Precio;
                    dal.AgregarPedidoBS(Nuevo);

                    trx.Complete();
                }
            }
            catch
            {
                throw new BLLException("Error al validar el pedido.");
            }




        }
    }
}


