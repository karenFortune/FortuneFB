using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortuneSystem.Models.Almacen
{
    public class salidas
    {
        [Display(Name = "#")]
        public int id_salida { get; set; }
        [Display(Name = "PO")]
        public string po { get; set; }
        [Display(Name = "DATE")]
        public string fecha { get; set; }
        [Display(Name = "DATE REQUESTED")]
        public string fecha_requested { get; set; }
        [Display(Name = "REQUESTED BY")]
        public string responsable { get; set; }
        [Display(Name = "SEAL")]
        public int sello { get; set; }
        [Display(Name = "TOTAL ITEMS")]
        public int total { get; set; }
        [Display(Name = "USER ID")]
        public int id_usuario { get; set; }
        [Display(Name = "USER")]
        public string usuario { get; set; }
        [Display(Name = "FROM ID")]
        public int id_origen { get; set; }
        [Display(Name = "FROM")]
        public string origen { get; set; }
        [Display(Name = "TO ID")]
        public int id_destino { get; set; }
        [Display(Name = "TO")]
        public string destino { get; set; }
        [Display(Name = "PO ID")]
        public int id_pedido { get; set; }
        [Display(Name = "ENTREGA")]
        public string id_entrega { get; set; }
        [Display(Name = "STATUS")]
        public int estado_aprobacion { get; set; }
        [Display(Name = "READY?")]
        public int estado_entrega { get; set; }
        [Display(Name = "SHIPPING")]
        public string id_envio { get; set; }
        [Display(Name = "REQUESTED DATE")]
        public string fecha_solicitud { get; set; }
        [Display(Name = "DRIVER")]
        public string driver { get; set; }
        [Display(Name = "# PALLET")]
        public string pallet { get; set; }
        public string sucursal { get; set; }
        public string po_number { get; set; }
        public string direccion_origen { get; set; }
        public string direccion_destino { get; set; }

        public string recibio { get; set; }
        public string auto { get; set; }
        public string placas { get; set; }
        public int id_sello { get; set; }
        public int id_sucursal { get; set; }

        public virtual salidas_item si { get; set; }
        public List<salidas_item> lista_salidas_item { get; set; }
    }
}