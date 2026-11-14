using System;
using System.Collections.Generic;
using System.Text;

namespace BackEntity
{
    public class DetallePedidoEntity
    {
        public int IDDetallePedido { get; set; }
        public int IDpedido { get; set; }   
        public int IDproducto { get; set; }
        public int Cantidad { get; set; }
        public List<DetallePedidoEntity> Detalles { get; set; } = new List<DetallePedidoEntity>();
    }
}
