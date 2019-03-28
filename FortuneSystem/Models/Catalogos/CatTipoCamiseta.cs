using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTipoCamiseta
    {
        [Display(Name = "TYPE T-SHIRT#")]
        public int IdTipo { get; set; }
        [Display(Name = "T-SHIRT CODE")]
        public string TipoProducto { get; set; }
        [Display(Name = "DESCRIPTION")]
        public string DescripcionTipo { get; set; }
        [Display(Name = "TYPE GROUP")]
        public string TipoGrupo { get; set; }
    }
}