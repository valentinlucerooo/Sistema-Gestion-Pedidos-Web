using System;
using System.Collections.Generic;
using System.Text;
using BackEntity;
using Microsoft.Data.SqlClient;

namespace BackMappers
{
    public static class EnvioMapper
    {
        public static EnvioEntity map(SqlDataReader lector)
        {
            EnvioEntity nuevo = new EnvioEntity();
            nuevo.IDEnvio = Convert.ToInt32(lector["ID_ENVIO"]);
            nuevo.Zona = lector["ZONA"].ToString();
            nuevo.Precio = Convert.ToDecimal(lector["PRECIO"]);
            return nuevo;
        }
    }
}
