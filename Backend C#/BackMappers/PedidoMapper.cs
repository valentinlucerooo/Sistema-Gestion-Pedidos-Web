using BackEntity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackMappers
{
    public static class PedidoMapper
    {
        public static PedidoEntity map(ProductoEntity producto, EnvioEntity envio, SqlDataReader lector)
        {
            PedidoEntity nuevo = new PedidoEntity();
            nuevo.IDpedido = Convert.ToInt32(lector["ID_PEDIDO"]);
            nuevo.Envio = envio;
            nuevo.MontoTotal = Convert.ToDecimal(lector["MONTO_TOTAL"]);
            nuevo.Prioridad = lector["PRIORIDAD"].ToString();
            nuevo.FechaRegistro = Convert.ToDateTime(lector["FECHA_REGISTRO"]);

            return nuevo;
        }
    }
}
