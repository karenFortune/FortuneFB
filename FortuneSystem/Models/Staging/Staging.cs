using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using FortuneSystem.Models.Almacen;

namespace FortuneSystem.Models.Staging
{
    /*public class Recibo {
        [Display(Name = "RECIBO")]
        public int id_recibo { get; set; }//PARA LAS TABLAS
        [Display(Name = "# PEDIDO")]
        public string id_po { get; set; }
        [Display(Name = "TOTAL")]
        public int total { get; set; }        
    }*/
    public class Staging
    {
        [Display(Name = "RECIBO")]
        public int id_recibo{ get; set; }//PARA LAS TABLAS
        [Display(Name = "# PEDIDO")]
        public int id_pedido { get; set; }
        [Display(Name = "STAGING")]
        public int id_staging { get; set; }
        [Display(Name = "FECHA ")]
        public string fecha { get; set; }
        [Display(Name = "USUARIO")]
        public int id_usuario { get; set; }
        [Display(Name = "TALLA")]
        public int id_talla { get; set; }
        [Display(Name = "PAIS")]
        public string pais{ get; set; }
        [Display(Name = "COLOR")]
        public int color { get; set; }
        [Display(Name = "PORCENTAJE")]
        public string porcentaje { get; set; }
        [Display(Name = "USER")]
        public int usuario { get; set;}
        [Display(Name = "ORDEN")]
        public string po { get; set; }
        [Display(Name = "TOTAL")]
        public int total { get; set; }
        [Display(Name = "IDSIZE")]
        public int id_size { get; set; }
        [Display(Name = "TALLA")]
        public string talla { get; set; }
        [Display(Name = "ESTILO")]
        public string estilo { get; set; }
        [Display(Name = "IDPO")]
        public int id_po_summary { get; set; }
        [Display(Name = "COLOR")]
        public string color_description { get; set; }
        [Display(Name = "USUARIO")]
        public string nombre_usuario { get; set; }
        public string usuario_conteo { get; set; }
    }

    public class pedido_staging {
        public int id_pedido { get; set; }
        public int id_summary{ get; set; }
        public string po { get; set; }
        public string customer { get; set; }
        public int id_customer { get; set; }
        public int id_customer_final { get; set; }
        public string customer_final { get; set; }
        public List<recibo> lista_recibo { get; set; }
        public int total { get; set; }
        public string estilo { get; set; }
        public string estilo_nombre { get; set; }
        public int id_estilo { get; set; }
        public int id_inventario{ get; set; }
        public string descripcion { get; set; }
        public string fecha { get; set; }
        public string turno { get; set; }
        public string tipo_stag { get; set; }
    }

    public class stag_conteo {
        public int id_staging { get; set; }
        public string po { get; set; }
        public string estilo { get; set; }
        public string estilo_nombre { get; set; }
        public string color { get; set; }
        public string talla { get; set; }
        public string porcentaje { get; set; }
        public string pais { get; set; }
        public string cantidad { get; set; }
        public string usuario { get; set; }
        public string usuario_conteo { get; set; }
        public string fecha { get; set; }
        public string observaciones { get; set; }
        public string turno { get; set; }
    }

    public class StagingD {
        public string talla { get; set; }
        public int id_talla { get; set; }
        public int total { get; set; }

    }
    public class Talla_staging{
        public int id_summary { get; set; }
        public int id_pedido { get; set; }
        public int id_estilo{ get; set; }
        public string talla { get; set; }
        public int id_talla { get; set; }
        public int total { get; set; }
        public int piezas { get; set; }
    }

    public class StagingDatos
    {
        public string Pais { get; set; }
        public string Porcentaje { get; set; }
        public string NombreColor { get; set; }

    }




}