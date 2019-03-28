using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Items
{
    public class CatTypePackItem
    {
        public int IdPackStyle { get; set; }
        [Display(Name = "DESCRIPTION PACK")]
        public string DescripcionPack { get; set; }
        public int IdSummary { get; set; }
    
    }
}