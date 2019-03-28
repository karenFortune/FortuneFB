using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTallaItem
    {
        [Display(Name = "SIZE#")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter the size.")]
        [Display(Name = "SIZE")]
        public string Talla { get; set; }
        [Display(Name = "ORDER")]
        public int Orden { get; set; }
    }
}