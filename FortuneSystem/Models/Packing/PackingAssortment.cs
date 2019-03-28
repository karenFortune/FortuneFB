using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Packing
{
    public class PackingAssortment
    {
        public int IdPackingAssort { get; set; }
        [Display(Name = "CARTONS#")]
        public int CantCartons { get; set; }
        [Display(Name = "TURN")]
        public Turnos Turnos { get; set; }
        public int IdTurno { get; set; }
        public int TipoTurno { get; set; }
        public int Usuario { get; set; }
        public string NombreUsr { get; set; }
        public int UsuarioModif { get; set; }
        public string NombreUsrModif { get; set; }
        [Display(Name = "TOTAL PIECES")]
        public int TotalPiezas { get; set; }
        public int IdBatch { get; set; }
        public string PackingName { get; set; }
        public int IdBlock { get; set; }
        public int IdPedido { get; set; }
        [Display(Name = "TOTAL MISSING PIECES")]
        public int TotalPiezasFalt { get; set; }
        [Display(Name = "TOTAL MISSING CARTONS")]
        public int TotalCartonesFalt { get; set; }
        public List<PackingAssortment> BatchAssort { get; set; }


    }

    public enum Turnos
    {
        First = 1,
        Second = 2
    }
}