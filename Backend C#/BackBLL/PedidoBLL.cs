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
               return  dal.ObtenerPedidosDEUsuario(idUsuario);
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
        public void ValidarBorrador(List<PedidoEntity> borrador ,List<DetallePedidoEntity> detalles)
        {
            try
            {
                if (borrador == null || borrador.Count == 0)
                    throw new BLLException("el borrador no puede estar vacio.");
                using (var trx = new TransactionScope())
                {
                    foreach (var pedido in borrador)
                    {
                        dal.InsertarPedidoCompleto(pedido, detalles);
                    }
                    foreach (var pedido in borrador)
                    {
                        dal.InsertarPedidoCompleto(pedido, detalles);
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
        public void ValidarCambioEstado(int ID, string nuevoEstado)
        {
            try
            {
                if (ID <= 0)
                {
                    throw new BLLException("El ID del pedido no es válido.");
                }
                if (nuevoEstado == "")
                {
                    throw new BLLException("El nuevo estado no puede estar vacío.");
                }
                dal.AgregarEstado(ID, nuevoEstado);
            }
            catch
            {
                throw new BLLException("Error al cambiar el estado del pedido.");
            }
        }

        
          public int ValidarPedido(PedidoEntity nuevo, List<DetallePedidoEntity> detalles)
        {
            try
            {
                // 1. Validaciones de negocio antes de abrir la transacción
                if (detalles == null || detalles.Count == 0)
                {
                    throw new BLLException("El pedido debe contener al menos un producto.");
                }

                // Validamos cada detalle (producto y cantidad)
                foreach (var detalle in detalles)
                {
                    if (detalle.IDproducto <= 0)
                    {
                        throw new BLLException("El identificador del producto no es válido.");
                    }
                    if (detalle.Cantidad <= 0)
                    {
                        throw new BLLException("La cantidad del producto debe ser mayor a cero.");
                    }
                }

                if (nuevo.Envio.Zona =="")
                {
                    throw new BLLException("Debe seleccionar una zona de envío válida.");
                }

                if (nuevo.Prioridad == null || nuevo.Prioridad.Length < 4)
                {
                    throw new BLLException("La prioridad debe tener al menos 4 caracteres.");
                }

                // 2. Transacción para asegurar que se guarde todo junto
                using (var trx = new TransactionScope())
                {
                   int idGenerado = dal.InsertarPedidoCompleto(nuevo, detalles);

                    trx.Complete();
                    return idGenerado;
                }
            }
            catch (Exception ex)
            {
                // Es útil propagar el mensaje original para saber si falló por una regla de negocio o por la BD
                throw new BLLException("Error al registrar el pedido: " + ex.Message);
            }
        }



    }
    
}


