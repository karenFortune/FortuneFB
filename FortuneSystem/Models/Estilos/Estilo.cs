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

namespace FortuneSystem.Models.Estilos
{
    public class Estilo
    {
        [Display(Name = "ID#")]
        public string id_po_summary { get; set; }
        [Display(Name = "ITEM ID")]
        public string id_estilo { get; set; }
        [Display(Name = "ITEM")]
        public string estilo { get; set; }
        [Display(Name = "COLOR CODE")]
        public string id_color { get; set; }
        [Display(Name = "COLOR")]
        public string color { get; set; }
        [Display(Name = "QUANTITY")]
        public double cantidad { get; set; }
        [Display(Name = "PRICE")]
        public double precio { get; set; }
        [Display(Name = "TOTAL PRICE")]
        public double total_precio { get; set; }
        [Display(Name = "GENDER")]
        public string genero { get; set; }
        public string stag { get; set; }
        public int totales { get; set; }
        public int totales_recibidos { get; set; }
    }
}