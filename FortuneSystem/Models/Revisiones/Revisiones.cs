using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Revisiones
{
    public class Revision
    {
        [Display(Name = "#")]
        public int Id {get; set;}
        [Display(Name = "Id Pedido")]
        public int IdPedido { get; set; }
        [Display(Name = "Id Revision")]
        public int IdRevisionPO { get; set; }
        [RegularExpression("^[0-9]{4}-[0-1][0-9]-[0-3][0-9]$", ErrorMessage = "Formato de fecha incorrecta.")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        [Display(Name = "REVISION DATE")]
        public DateTime FechaRevision { get; set; }
        [Display(Name = "Id Estado")]
        public int IdStatus { get; set; }

    }
}