using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.RecepcionPO
{
    public class ResumenPO
    {
        public int IdPO { get; set; }
        public int IdItem { get; set; }
        public int IdColor { get; set; }
        public int Qty { get; set; }
        public float precio { get; set; }
        public int IdPedido { get; set; }
    }
}