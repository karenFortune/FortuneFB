using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTipoOrden
    {
        [Display(Name = "TYPE ORDEN #")]
        public int IdTipoOrden { get; set; }

        [Display(Name = "TYPE ORDEN")]
        public string TipoOrden { get; set; }
    }
}