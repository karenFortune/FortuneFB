using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatSucursal
    {
        [Display(Name = "FACTORY#")] 
        public int IdSucursal { get; set; }

        [Required(ErrorMessage = "Please enter the customer's name.")]
        [Display(Name = "FACTORY")]
        [Column("sucursal")]

        public string Sucursal { get; set; }
        [Display(Name = "ADDRESS")]
        public string Direccion { get; set; }


    }
}