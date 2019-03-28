using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortuneSystem.Models.Almacen
{
    public class Item
    {
        [Display(Name = "#")]       
        public int id_item { get; set; }
        public int id_color { get; set; }
        public int id_fabricante { get; set; }
        public int id_size { get; set; }
        [Display(Name = "Description")]
        public string descripcion { get; set; }
        public int id_body_type { get; set; }
        public int id_gender { get; set; }
        public int id_fabric_percent { get; set; }
        public int id_yarn { get; set; }
        public string division { get; set; }
        [Display(Name = "Item")]
        public string item_nombre { get; set; }
        [Display(Name = "Fabric")]
        public string fabric_type { get; set; }
       
        public int id_fabric_type { get; set; }
        [Display(Name = "Color")]
        public string color { get; set; }
        [Display(Name = "Manufacturer")]
        public string fabricante { get; set; }
        [Display(Name = "Size")]
        public string size { get; set; }
        [Display(Name = "Body type")]
        public string body_type { get; set; }
        [Display(Name = "Gender")]
        public string gender { get; set; }
        [Display(Name = "Percents")]
        public string fabric_percent { get; set; }
        [Display(Name = "Yarn")]
        public string yarn { get; set; }


    }
}