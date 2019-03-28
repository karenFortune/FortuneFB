using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatColores
    {
        [Display(Name = "NO.COLOR")]
        public int IdColor { get; set; }

        [Required]
        [Display(Name = "COLOR CODE")]
        public string CodigoColor { get; set; }

        [Required]
        [Display(Name = "DESCRIPTION")]
        public string DescripcionColor { get; set; }
    }
}