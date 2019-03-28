using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Almacen
{
    public class salidas_item
    {
        public int id_salida { get; set; }
        public int id_salida_item { get; set; }
        public int id_inventario { get; set; }//<------------------
        public int cantidad { get; set; }        
        public string estilo { get; set; }
        public int  id_pedido { get; set; }
        public int id_estilo { get; set; }

        public Inventario item { get; set; }
        public List<Inventario> lista_inventario { get; set; }
        
        public string descripcion { get; set; }
        public string codigo { get; set; }
        public string po { get; set; }
        public string mill_po { get; set; }
        public string mp_number { get; set; }
        public string color { get; set; }
        public int summary { get; set; }
        public int cajas { get; set; }
        public string po_number { get; set; }

        public int total_inventario { get; set; }
        
    }
}