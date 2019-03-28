using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatClienteFinal
    {
        [Display(Name = "CUSTOMER ORDER#")]
        public int CustomerFinal { get; set; }
        [Required(ErrorMessage = "Please enter the customer's order name.")]
        [Display(Name = "CUSTOMER ORDER")]
        public string NombreCliente{ get; set; }
    }
}