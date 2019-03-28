using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Arte
{
    public class ImagenArte
    {
        public int IdImgArte { get; set; }
        public int IdSummary { get; set; }
        [Display(Name = "Imagen Arte"), DataType(DataType.Upload)]
        public byte[] ImgArte { get; set; }
        [Display(Name = "Imagen PNL"), DataType(DataType.Upload)]
        public byte[] ImgPNL { get; set; }
        public int IdUPC { get; set; }

        public string Estilo { get; set; }
    }
}