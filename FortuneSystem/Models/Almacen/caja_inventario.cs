using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Almacen
{
    public class caja_inventario
    {
        public int id_recibo_item { get; set; }
        public int id_caja { get; set; }
        public int id_inventario { get; set; }
        public int cantidad_inicial { get; set; }
        public int cantidad_restante { get; set; }
        public string img { get; set; }

    }
}