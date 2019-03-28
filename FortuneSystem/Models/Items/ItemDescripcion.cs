using FortuneSystem.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Items
{
    public class ItemDescripcion
    {
        public int ItemId { get; set; }
        [Display(Name = "ITEM")]
        public string ItemEstilo { get; set; }
        [Display(Name = "DESCRIPTION")]
        public string Descripcion { get; set; }

        public int IdSummary { get; set; }
        public virtual CatColores CatColores { get; set; }
    }
}