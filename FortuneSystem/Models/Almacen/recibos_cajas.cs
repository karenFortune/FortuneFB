using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Almacen
{
    public class recibos_cajas
    {
        public int id_recibo { get; set; }
        public int id_recibo_item { get; set; }
        public int id_recibo_caja { get; set; }
        public int total_cajas { get; set; }
        public int cantidad_caja { get; set; }
        public int id_talla { get; set; }
    }
}