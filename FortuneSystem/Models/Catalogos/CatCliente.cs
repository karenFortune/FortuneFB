using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatCliente
    {
        [Display(Name = "CUSTOMER#")] 
        public int Customer { get; set; }

        [Required(ErrorMessage = "Please enter the customer's name.")]
        [Display(Name = "CUSTOMER")]
        [Column("NAME")]
        public string Nombre { get; set; }
    }
}