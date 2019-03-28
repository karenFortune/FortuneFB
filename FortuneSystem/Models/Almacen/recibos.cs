using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FortuneSystem.Models.Almacen
{
    public class recibo
    {
        public int id_recibo { get; set; }
        public string fecha { get; set; }
        public int total { get; set; }
        public int id_usuario { get; set; }
        public int id_sucursal { get; set; }
        public int id_origen { get; set; }
        public string items { get; set; }
        public string mp_number { get; set; }
        public string usuario { get; set; }
        public string sucursal { get; set; }
        public string mill_po { get; set; }
        public string po_referencia { get; set; }
        public virtual recibos_item ri { get; set; }
        public List<recibos_item> lista_recibos_item { get; set; }

    }
}