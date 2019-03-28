using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatComentarios
    {
        public int IdComentario { get; set; }
        [Display(Name = "COMMENTS")]
        public string Comentario { get; set; }
        [RegularExpression("^[0-1][0-9][- /.][0-3][0-9][- /.][0-9]{4}$", ErrorMessage = "Incorrect date format.")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        [Display(Name = "DATE")]
        public DateTime FechaComentario { get; set; }
        public string FechaComents { get; set; }
        public int IdUsuario { get; set; }
        public int IdSummary { get; set; }
        public string NombreUsuario { get; set; }
    }
}