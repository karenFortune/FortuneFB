using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Almacen
{
    public class recibos_item
    {
        public int id_recibo { get; set; }
        public int id_recibo_item { get; set; }
        public int id_inventario { get; set; }
        public int total { get; set; }

        public Inventario item { get; set; }

        public virtual caja_inventario ci { get; set; }
        public List<caja_inventario> lista_cajas { get; set; }
    }
}