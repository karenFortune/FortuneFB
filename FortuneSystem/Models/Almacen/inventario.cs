using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortuneSystem.Models.Almacen
{
    public class Inventario
    {
        [Display(Name = "#")]
        public int id_inventario { get; set; }
        public int id_sucursal { get; set; }
        [Display(Name = "OFFICE")]
        public string sucursal { get; set; }
        public int id_recibo { get; set; }
        public int id_pedido { get; set; }
        [Display(Name = "ORDER")]
        public string po { get; set; }
        public int id_pais { get; set; }
        [Display(Name = "COUNTRY")]
        public string pais { get; set; }
        public int id_fabricante { get; set; }
        [Display(Name = "MANUFACTURER")]
        public string fabricante { get; set; }


        public int id_categoria_inventario { get; set; }
        [Display(Name = "CATEGORY")]
        public string categoria_inventario { get; set; }
        public int id_color { get; set; }
        [Display(Name = "COLOR")]
        public string color { get; set; }
        public int id_body_type { get; set; }
        [Display(Name = "BODY TYPE")]
        public string body_type { get; set; }
        public int id_genero { get; set; }
        [Display(Name = "GENDER")]
        public string genero { get; set; }
        public int id_fabric_type { get; set; }
        [Display(Name = "FABRIC TYPE")]
        public string fabric_type { get; set; }
        [Display(Name = "FABRIC 100%")]
        public string fabric_percent { get; set; }
        public int id_fabric_percent { get; set; }
        public int id_location { get; set; }
        public string location { get; set; }
        [Display(Name = "QUANTITY")]
        public int total { get; set; }
        public int id_size { get; set; }
        [Display(Name = "SIZE")]
        public string size { get; set; }
        public int id_customer { get; set; }
        [Display(Name = "CUSTOMER")]
        public string customer { get; set; }
        public int id_final_customer { get; set; }
        [Display(Name = "CUSTOMER FINAL")]
        public string final_customer { get; set; }
        public int id_estado { get; set; }
        [Display(Name = "STOCK")]
        public string estado { get; set; }
        public int minimo { get; set; }
        [Display(Name = "NOTES")]
        public string notas { get; set; }
        [Display(Name = "")]
        public string stock { get; set; }
        [Display(Name = "DATE COMMENT")]
        public string date_comment { get; set; }
        [Display(Name = "COMMENT")]
        public string comment { get; set; }
        [Display(Name = "PURCHASED FOR")]
        public string purchased_for { get; set; }
        [Display(Name = "FAMILY")]
        public string family_trim { get; set; }
        public string id_family_trim { get; set; }
        [Display(Name = "UNIT")]
        public string unit { get; set; }
        public string id_unit { get; set; }
        [Display(Name = "DESCRIPTION")]
        public string descripcion { get; set; }
        public string trim { get; set; }
        public int id_trim { get; set; }
        [Display(Name = "DATE")]
        public string fecha_ultimo_recibo { get; set; }
        public int diferencia { get; set; }
        public int id_estilo { get; set; }
        public virtual salidas si { get; set; }
        public List<salidas> lista_salidas { get; set; }
        public string amt_item { get; set; }
        public string codigo_color { get; set; }
        //para reportes de trasnferencias
        public string mp_number { get; set; }
        public string mill_po { get; set; }
        public string po_reference { get; set; }
        public string estilo { get; set; }
        public string fecha { get; set; }
        public int auditoria { get; set; }
        public int id_item { get; set; }


    }

    public class lugares {
        public int id_lugar { get; set; }
        public string lugar { get; set; }
    }



}