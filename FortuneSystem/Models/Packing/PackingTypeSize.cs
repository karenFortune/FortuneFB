using FortuneSystem.Models.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Packing
{
    public class PackingTypeSize
    {
        public int IdPackingTypeSize { get; set; }
        public int IdTalla { get; set; }
        [Display(Name = "PIECES ")]
        public int Pieces { get; set; }
        public int Ratio { get; set; }
        public int IdSummary { get; set; }
        public int IdTipoEmpaque { get; set; }
        public string NombreTipoPak{ get; set; }
        public string Talla { get; set; }
        public string TallasGrl { get; set; }
        public string Ratios { get; set; }
        [Display(Name = "TYPE OF PACKAGING ")]
        public TipoEmpaque TipoEmpaque { get; set; }
        [Display(Name = "PACKAGING FORM")]
        public FormaEmpaque FormaEmpaque { get; set; }
        public int IdFormaEmpaque { get; set; }
        [Display(Name = "PO#")]
        public int NumberPO { get; set; }
        [Display(Name = "QTY")]
        public int Cantidad { get; set; }
        [Display(Name = "CARTONS")]
        public int Cartones { get; set; }
        public int PartialNumber { get; set; }
        public int TotalRatio { get; set; }
        [Display(Name = "TOTAL PIECES")]
        public int TotalPieces { get; set; }
        [Display(Name = "TOTAL UNITS")]
        public int TotalUnits { get; set; }
        [Display(Name = "TOTAL CARTONS")]
        public int TotalCartones { get; set; }
        public virtual PackingM PackingM { get; set; }
        public List<PackingTypeSize> ListaEmpaque { get; set; }
        [Display(Name = "PACKING NAME")]
        public string PackingName { get; set; }
        [Display(Name = "ASSORT NAME")]
        public string AssortName { get; set; }
        [Display(Name = "PACKING")]
        public string PackingRegistrado { get; set; }
        public int IdBlockPack { get; set; }
        public virtual ItemDescripcion ItemDescripcion { get; set; }
        public int NumRegistros { get; set; }
        public int TotalPiezas { get; set; }




    }

    public enum TipoEmpaque
    {
        BULK = 1,
        PPK = 2
        //ASSORTMENT = 3
    }

    public enum FormaEmpaque
    {
        STORE = 1,
        ECOM = 2
        //ASSORTMENT = 3
    }
}