using System;
using System.Collections.Generic;
using System.Text;

namespace BackEntity
{
    public class ProductoEntity
    {
        public int IDProducto { get; set; }
        public string ProductoTipo { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public override string ToString() // Para mostrar la descripción en el ComboBox
        {
            return Descripcion;
        }
    }
}
