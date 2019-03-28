using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Packing
{
    public class PackingSize
    {
        public int IdPackingSize { get; set; }
        public int IdTalla { get; set; }
        public string Talla { get; set; }
        public int IdSummary { get; set; }
        public int Calidad { get; set; }

    }

}