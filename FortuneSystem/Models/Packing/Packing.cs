using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Packing
{
    public class PackingM
    {
        public int IdPacking { get; set; }
        public int IdSummary { get; set; }
        [Display(Name = "SIZE")]
        public string Talla { get; set; }
        public int IdTalla { get; set; }
        [Display(Name = "BOX#")]
        public int CantBox { get; set; }
        [Display(Name = "TURN")]
        public Turno Turnos { get; set; }
        public int IdTurno { get; set; }
        public int TipoTurno { get; set; }
        public int Usuario { get; set; }
        public string NombreUsr { get; set; }
        public int UsuarioModif { get; set; }
        public string NombreUsrModif { get; set; }
        public int TotalPiezas { get; set; }
        public int IdBatch { get; set; }
        public int IdPackingSize { get; set; }
        public int IdPackingTypeSize { get; set; }
        public int TipoEmpaque { get; set; }
        public int NumberPO { get; set; }
        public int CantidadP { get; set; }
        public int Partial { get; set; }
        [Display(Name = "PPKS#")]
        public int CantidadPPKS { get; set; }

        [Display(Name = "TOTAL CARTONS#")]
        public int TotalCartonsPPK { get; set; }

        [Display(Name = "MISSING CARTONS#")]
        public int TotalCartonesFaltPPK { get; set; }

        public List<PackingM> Batch { get; set; }

        public List<PackingTypeSize> ListEmpaque { get; set; }

        public virtual IMAGEN_ARTE ImagenArte { get; set; }

        public virtual PackingSize PackingSize { get; set; }

        public virtual PackingTypeSize PackingTypeSize { get; set; }

        public virtual PackingAssortment PackingAssort { get; set; }
    }

    public enum Turno
    {
        First = 1,
        Second = 2
    }
}