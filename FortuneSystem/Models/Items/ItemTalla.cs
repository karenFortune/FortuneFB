using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Item
{
    public class ItemTalla
    {
        public int Id { get; set; }
        [Display(Name = "SIZE")]
        public string Talla { get; set; }
        public int IdTalla { get; set; }
        [Display(Name = "QUANTITY")]
        public int Cantidad { get; set; }
        [Display(Name = "EXAMPLES")]
        public int Ejemplos { get; set; }
        [Display(Name = "EXTRA")]
        public int Extras { get; set; }
        public int IdSummary { get; set; }
        public string Estilo { get; set; }
        public string DescripcionEstilo { get; set; }
    }
}