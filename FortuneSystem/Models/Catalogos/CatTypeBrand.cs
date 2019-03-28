using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTypeBrand
    {
        [Display(Name = "#")]
        public int IdTipoBrand { get; set; }
        [Display(Name = "CODE BRAND")]
        public string CodigoBrand { get; set; }
        [Display(Name = "BRAND NAME")]
        public string TipoBrandName { get; set; }
    }
}