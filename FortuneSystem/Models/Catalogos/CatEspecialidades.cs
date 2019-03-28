using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatEspecialidades
    {
        [Display(Name = "ID SPECIALTY#")]
        public int IdEspecialidad { get; set; }
        [Display(Name = "SPECIALTY")]
        public string Especialidad { get; set; }
    }
}