using System;
using System.Collections.Generic;
using System.Text;
using BackEntity;
using Microsoft.Data.SqlClient;

namespace BackMappers
{
    public static class ProductoMapper
    {
        public static ProductoEntity map(SqlDataReader lector)
        {
            ProductoEntity nuevo = new ProductoEntity();
            nuevo.IDProducto = Convert.ToInt32(lector["ID_PRODUCTO"]);
            nuevo.ProductoTipo = lector["TIPO_PRODUCTO"].ToString();
            nuevo.Descripcion = lector["DESCRIPCION"].ToString();
            nuevo.Precio = Convert.ToDecimal(lector["PRECIO"]);
            return nuevo;
        }
    }
}
