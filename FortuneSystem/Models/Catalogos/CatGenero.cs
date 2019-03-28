using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatGenero
    {
        [Display(Name = "GENDER#")]
        public int IdGender { get; set; }

        [Required(ErrorMessage = "Please enter the gender")]
        [Display(Name = "GENDER")]
        public string Genero { get; set; }

        public virtual CatTallaItem CatTallaItem { get; set; }

        [Display(Name = "GENDER CODE")]
        public string GeneroCode { get; set; }

    }
}