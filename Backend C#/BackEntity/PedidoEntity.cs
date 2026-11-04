using System;
using System.Collections.Generic;
using System.Text;

namespace BackEntity
{
    public class PedidoEntity
    {
        public int IDpedido { get; set; }
        public UsuarioEntity Usuario{ get; set; }
        public ProductoEntity Producto { get; set; }
        public int Cantidad { get; set; }
        public EnvioEntity Envio { get; set; }
        public decimal MontoTotal { get; set; }
        public string Prioridad { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Tipo { get; set; }


    }
}
