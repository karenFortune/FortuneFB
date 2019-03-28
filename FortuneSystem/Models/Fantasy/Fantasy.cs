using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortuneSystem.Models.Fantasy{

    public class Cliente{
        public int id_cliente { get; set; }
        public string nombre { get; set; }
        public int estado { get; set; }
    }
    public class Estilo{
        public int id_estilo_fantasy { get; set; }
        public int id_estilo{ get; set; }
        public string estilo { get; set; }
        public string cliente { get; set; }
        public string descripcion { get; set; }
        public int id_cliente { get; set; }
        public int estado { get; set; }
        public int id_color { get; set; }
        public string color { get; set; }
        public List<InventarioFantasy> lista_inventario { get; set; }
    }

    public class Registro {
        public int id_registro { get; set; }
        public int id_estilo_fantasy { get; set; }
        public int id_estilo { get; set; }
        public int total { get; set; }
        public string fecha { get; set; }
        public string tipo { get; set; }
        public string estado { get; set; }
        public string ship_date { get; set; }
        public int id_packing_list { get; set; }
        public int id_pedido { get; set; }
        public string pedido { get; set; }
        public Estilo estilo { get; set; }
        public Cliente cliente { get; set; }
        public List<Cantidades> lista_cantidades { get; set; }
        public string packing { get; set; }
        
    }

    public class Cantidades {
        public int id_cantidad { get; set; }
        public int id_estilo { get; set; }
        public int id_registro { get; set; }
        public int total { get; set; }
        public int restante { get; set; }
        public string talla { get; set; }
    }

    public class InventarioFantasy {
        public int id_inventario { get; set; }
        public int id_estilo { get; set; }
        public int total { get; set; }
        public string talla { get; set; }
    }

















}
