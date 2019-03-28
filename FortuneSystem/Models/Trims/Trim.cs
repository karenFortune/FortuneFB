using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FortuneSystem.Models.Shipping;
using FortuneSystem.Models.Almacen;

namespace FortuneSystem.Models.Trims
{
    public class Trim {
        public Pedido_t po { get; set; }
        public Item_t item { get; set; }
        public Estilos_t estilo { get; set; }
        public string usuario { get; set; }
        public string customer { get; set; }
        public int tipo_empaque { get; set; }
        public List<ratio_tallas> lista_ratio { get; set; }
        public List<Estilos_t> lista_estilos { get; set; }
        public List<Estilos_t> lista_generos { get; set; }
        public int id_pedido { get; set; }
        public string pedido { get; set; }
    }
    public class Pedido_t {
        public int id_pedido { get; set; }
        public string pedido { get; set; }
        public List<Estilos_t> lista_estilos { get; set; }
        public int total { get; set; }
        public int cantidad { get; set; }
    }
    public class Estilos_t {
        public int id_estilo { get; set; }
        public int id_po_summary { get; set; }
        public string estilo { get; set; }
        public string descripcion { get; set; }
        public string genero { get; set; }
        public List<Talla_t> lista_tallas { get; set; }
        public List<Trim_requests> lista_requests { get; set; }
    }
    public class Talla_t {
        public int id_talla { get; set; }
        public int total { get; set; }
        public string talla { get; set; }
    }
    public class Item_t {
        public int id_item { get; set; }
        public int categoria { get; set; }
        public int total { get; set; }
        public int total_estilo { get; set; }
        public string descripcion { get; set; }
        public string componente { get; set; }
        public string fecha { get; set; }

    }
    public class Trim_inventario {
        public int id_inventario { get; set; }
        public int id_pedido { get; set; }
        public string pedido { get; set; }
        public int id_estilo { get; set; }
        public string estilo { get; set; }
        public string descripcion { get; set; }
        public int id_family_trim { get; set; }
        public string family_trim { get; set; }
        public int id_unit { get; set; }
        public string unit { get; set; }
        public int id_trim { get; set; }
        public string trim { get; set; }
        public int total { get; set; }
        public int id_item { get; set; }
        public string item { get; set; }
    }
    public class Trim_item {
        public int id_item { get; set; }
        public string item { get; set; }
        public string descripcion { get; set; }
        public string family { get; set; }
        public string unit { get; set; }
        public int minimo { get; set; }
        public int total { get; set; }
    }

    public class Trim_requests{
        public int id_request { get; set; }
        public int id_talla { get; set; }
        public int id_summary { get; set; }
        public int id_item { get; set; }
        public int total { get; set; }
        public int cantidad { get; set; }
        public int blanks { get; set; }
        public int restante { get; set; }
        public int revision { get; set; }
        public int id_usuario { get; set; }
        public int id_estilo{ get; set; }
        public string talla { get; set; }
        public string item { get; set; }
        public string usuario { get; set; }
        public string fecha { get; set; }
        public string estilo { get; set; }
        public string fecha_recibo { get; set; }
        public string tipo_item { get; set; }
        public string comentarios { get; set; }
        public int recibo { get; set; }
        public int id_pedido { get; set; }
        public int entregado { get; set; }
        public string pedido { get; set; }
        public int auditado { get; set; }
        public int id_estado { get; set; }
        public int id_inventario { get; set; }
        public string estado { get; set; }
        public recibo recibo_item { get; set; }
    }
    public class Estilos_trims { }
    public class Trim_entregas {
        public int id_entrega { get; set; }
        public int id_pedido{ get; set; }
        public string entrega { get; set; }
        public string recibe { get; set; }
        public string fecha { get; set; }
        public string pedido { get; set; }
        public List<Trim_requests> lista_request { get; set; }
    }
    public class Pedidos_trim {
        public int id_pedido { get; set; }
        public string pedido { get; set; }
        public int id_customer { get; set; }
        public string customer { get; set; }
        public string ship_date { get; set; }
        public int id_gender { get; set; }
        public string gender { get; set; }
        public List<Empaque> lista_empaque { get; set; }
        public List<Assortment> lista_assort { get; set; }
        public List<Family_trim> lista_families { get; set; }
    }

    public class Family_trim {
        public int id_family_trim { get; set; }
        public string family_trim { get; set; }
        public List<Trim_requests> lista_requests { get; set; }
    }










}

