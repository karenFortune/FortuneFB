using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatStatus
    {
        [Display(Name = "STATUS#")]
        public int IdStatus { get; set; }

        [Required(ErrorMessage = "Please enter the status name.")]
        [Display(Name = "STATUS")]
        [Column("ESTADO")]
        public string Estado { get; set; }

    }
}