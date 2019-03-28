using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.PrintShop
{
    public class PrintShopC
    {
        public int IdPrintShop { get; set; }
        public int IdSummary { get; set; }
        [Display(Name = "SIZE")]
        public string Talla { get; set; }
        public int IdTalla { get; set; }
        public int Printed { get; set; }
        public int MisPrint { get; set; }
        public int Defect { get; set; }
        public int Repair { get; set; }
        [Display(Name = "MACHINE")]
        public int Maquina { get; set; }
        public Maquina Maquinas { get; set; }
        public string NombreMaquina { get; set; }
        [Display(Name = "TURN")]
        public Turno Turnos { get; set; }
        public int TipoTurno { get; set; }
        public int Usuario { get; set; }
        public string NombreUsr { get; set; }
        public int UsuarioModif { get; set; }
        public string NombreUsrModif { get; set; }
        public int Total { get; set; }
        public int IdBatch { get; set; }
        [Display(Name = "STATE PALLET")]
        public Boolean EstadoPallet { get; set; }
        public string Status { get; set; }
        public int Cargo { get; set; }
        [Display(Name = "COMMENTS")]
        public string Comentarios { get; set; }

        public List<PrintShopC> Batch { get; set; }

        public virtual IMAGEN_ARTE ImagenArte { get; set; }
    }

    public enum Turno
    {
        First = 1,
        Second = 2
    }
    
    public enum Maquina
    {
        Machine1 = 1,
        Machine2 = 2,
        Machine3 = 3,
        Machine4 = 4,
        Machine5 = 5,
        Machine6 = 6,
        Machine7 = 7,
        Machine8 = 8,
        Machine9 = 9,
        Machine10 = 10,
        Machine11 = 11,
        Machine12 = 12
    }
}