using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTela
    {
        [Display(Name = "FABRIC#")]
        public int Id_Tela { get; set; }
        [Display(Name = "FABRIC")]
        public string Tela { get; set; }
        [Display(Name = "FABRIC CODE")]
        public string CodigoTela { get; set; }
    }
}