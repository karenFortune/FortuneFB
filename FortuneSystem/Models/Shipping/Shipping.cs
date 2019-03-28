using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortuneSystem.Models.Shipping
{
    public class Shipping {
        public int id_shipping { get; set; }
        public int id_pallet { get; set; }
        public int po_number { get; set; }
        public List<estilos> lista_estilos { get; set; }
        public int codigo { get; set; }

    }

    public class Breakdown {
        public int id_pedido { get; set; }
        public string po { get; set; }
        public int id_summary { get; set; }
        public string estilo { get; set; }
        public int id_estilo { get; set; }
        public int dc { get; set; }
        public string descripcion { get; set; }
        public int id_color { get; set; }
        public string codigo_color { get; set; }
        public int id_pallet { get; set; }
        public int total { get; set; }
        public int utilizado { get; set; }
    }

    public class Pk {
        public int id_packing_list { get; set; }
        public string packing { get; set; }
        public int id_direccion_envio { get; set; }
        public Direccion origen { get; set; }
        public Direccion destino { get; set; }
        public int id_customer { get; set; }
        public string customer { get; set; }
        public int id_customer_po { get; set; }
        public string customer_po { get; set; }
        public int id_pedido { get; set; }
        public string pedido { get; set; }
        public Drivers conductor { get; set; }
        public Container contenedor { get; set; }
        public string shipping_manager { get; set; }
        public string seal { get; set; }
        public string replacement { get; set; }
        public string fecha { get; set; }
        public string dc { get; set; }
        public string parte { get; set; }
        public string number_po { get; set; }
        public List<Tarima> lista_tarimas { get; set; }
        public string tipo { get; set; }
        public int tipo_empaque { get; set; }
        public int id_container { get; set; }
        public int num_envio { get; set; }
        public int id_driver { get; set; }
        public List<estilos> lista_estilos { get; set; }
        public string cancel_date { get; set; }

        public int total_po { get; set; }
        public int total_enviado { get; set; }

        public int total_tarimas { get; set; }
        public int total_cajas { get; set; }
        public int total_piezas { get; set; }
        public string siglas_cliente { get; set; }
        public List<Labels> lista_labels{ get; set; }
    }

    public class Tarima {
        public int id { get; set; }
        public int id_tarima { get; set; }
        public List<estilos> lista_estilos { get; set; }
        public List<Assortment> lista_assortments { get; set; }
    }

    public class Assortment {
        public int id_assortment { get; set; }
        public int cartones { get; set; }
        public string nombre { get; set; }
        
        public int block { get; set; }
        public List<estilos> lista_estilos { get; set; }
        public int pk { get; set; }
        public int id_summary { get; set; }
        public int id_talla { get; set; }
        public int ratio { get; set; }
    }

    public class estilos {
        public int id { get; set; }
        public int id_po_summary { get; set; }
        public int id_estilo { get; set; }
        public int id_color { get; set; }
        public string color { get; set; }
        public string estilo { get; set; }
        public int cajas { get; set; }
        public string descripcion { get; set; }
        
        public int boxes { get; set; }
        public int number_po { get; set; }
        public string descripcion_final { get; set; }
        public List<ratio_tallas> lista_ratio { get; set; }
        
        public int id_shipping_id { get; set; }
        public int pk { get; set; }
        public string tipo { get; set; }
        public string store { get; set; }
        public int id_talla { get; set; }
        public string talla { get; set; }
        public string description { get; set; }
        public int piezas { get; set; }
        public List<Cantidades_Estilos> lista_cantidades { get; set; }

        public string dc { get; set; }
        
        public string ext { get; set; }
        public int tipo_empaque { get; set; }
        public int index_dc { get; set; }
        public int usado{ get; set; }
        public Assortment assort { get; set; }
        public string assort_nombre { get; set; }

    }

    public class ratio_tallas {
        public int id_estilo { get; set; }
        public int ratio { get; set; }
        public int piezas { get; set; }
        public int id_talla { get; set; }
        public string talla { get; set; }
        public int total_talla { get; set; }

        public int id_porcentaje { get; set; }
        public string porcentaje { get; set; }
        public int id_pais { get; set; }
        public string pais { get; set; }
        public int cantidad { get; set; }

        public int total { get; set; }
        public int ejemplos { get; set; }
        public int extras { get; set; }
        public string tipo { get; set; }

        public int id_packing_assort { get; set; }
        public string packing_name { get; set; }
        public string assort_name { get; set; }
        public int tipo_empaque { get; set; }

    }

    public class recibo_fantasy {
        public string nombre_entrega { get; set; }
        public string nombre_recibe { get; set; }
        public string po { get; set; }
        public int id_pedido { get; set; }
        public int id_estilo { get; set; }
        public string estilo { get; set; }
        public string descripcion { get; set; }
        public int id_color { get; set; }
        public string color { get; set; }
        public int id_talla { get; set; }
        public string talla { get; set; }
        public int piezas { get; set; }
        public int cajas { get; set; }
        public int tipo_empaque { get; set; }
        public List<ratio_tallas> lista_ratio { get; set; }
    }



    public class estilo_shipping{
        public string nombre_entrega { get; set; }
        public string nombre_recibe { get; set; }
        public string po { get; set; }
        public int id_pedido { get; set; }
        public int id_estilo { get; set; }
        public string estilo { get; set; }
        public string descripcion { get; set; }
        public int id_summary{ get; set; }
        public int id_color { get; set; }
        public string color { get; set; }
        public int id_talla { get; set; }
        public string talla { get; set; }
        public int piezas { get; set; }
        public int cajas { get; set; }
        public int tipo_empaque { get; set; }
        public List<ratio_tallas> lista_ratio { get; set; }
        public List<Empaque> lista_empaque { get; set; }

    }
    public class Empaque{

        public int tipo_empaque { get; set; }
        public List<ratio_tallas> lista_ratio { get; set; }

        public int id_packing_assort { get; set; }
        public string packing_name { get; set; }        
        public string assort_name { get; set; }        
    }


    public class Drivers {
        public int id_driver { get; set; }
        public string carrier { get; set; }
        public string driver_name { get; set; }
        public string plates { get; set; }
        public string scac { get; set; }
        public string caat { get; set; }
        public string tractor { get; set; }
    }
    public class Direccion {
        public int id_direccion { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public string ciudad { get; set; }
        public string zip { get; set; }
    }
    public class Container {
        public int id_container { get; set; }
        public string eco { get; set; }
        public string plates { get; set; }
    }
    public class Labels {
        public int id_label { get; set; }
        public string label { get; set; }
        public string tipo { get; set; }
    }

    public class Fabricantes {
        public int id_pais { get; set; }
        public double porcentaje { get; set; }
        public string pais { get; set; }
        public int cantidad { get; set; }
        public string percent { get; set; }
    }

    public class Cantidades_Estilos {
        public int id_pedido { get; set; }
        public int id_summary { get; set; }
        public int id_estilo { get; set; }
        public int id_assort { get; set; }
        public List<Talla> lista_tallas { get; set; }
        public int cantidad_pedido { get; set; }
        public int total_pedido { get; set; }
        public int total_enviado { get; set; }
    }

    public class Talla{
        public int id_talla{ get; set; }
        public string talla { get; set; }
        public int total { get; set; }
        public int ejemplos { get; set; }
        public int extras { get; set; }
        public int ratio { get; set; }
        public int assortment { get; set; }
    }
    
    public class Estilo_Pedido {
        public int id_estilo { get; set; }
        public int id_summary  { get; set; }
        public string estilo { get; set; }
        public string descripcion { get; set; }
        public int id_color { get; set; }
        public string color { get; set; }
        public List<Packing_Estilo> lista_pk { get; set; }
        public List<Talla> totales_pedido { get; set; }

    }
    public class Packing_Estilo {
        public int id_packing { get; set; }
        public int id_shipping { get; set; }
        public string package { get; set; }
        public string fecha { get; set; }
        public string tipo { get; set; }
        public List<Talla> lista_enviados { get; set; }
    }

    public class Estilo_PO {
        public int id_estilo { get; set; }
        public int id_pedido { get; set; }
        public int id_summary { get; set; }
        public string estilo { get; set; }
        public string pedido { get; set; }
        public string fecha { get; set; }
        public string descripcion { get; set; }
        public int id_color { get; set; }
        public string color { get; set; }
        public string estado { get; set; }
        public int total { get; set; }
    }

    public class Shipping_pk {
        public int id_packing { get; set; }
        public int id_pedido{ get; set; }
        public string packing { get; set; }
        public string pedido { get; set; }
        public string destino { get; set; }
        public string fecha { get; set; }
        public int piezas { get; set; }
        public int cajas { get; set; }
        public int pallets { get; set; }
        public int num_envio { get; set; }

    }


    public class Po {
        public int id_customer { get; set; }
        public string customer { get; set; }
        public int id_customer_po { get; set; }
        public string customer_po { get; set; }
        public int id_pedido { get; set; }
        public string pedido { get; set; }
        public int estilos { get; set; }
        public int total { get; set; }
        public string fecha_cancelacion { get; set; }
        public string estado { get; set; }
    }

    






















}